using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using System.Net.Http.Headers;
using Octokit;

namespace HookHook.Backend.Reactions
{
    public class GithubCreateRepository : IReaction
    {
        public string RepositoryName {get; private init;}

        public string Description {get; private init;}

        public GitHubClient _githubClient;
        private readonly HttpClient _httpClient = new();

        public GithubCreateRepository(string repositoryName, string description)
        {
            RepositoryName = repositoryName;
            Description = description;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }


        public async Task Execute(Entities.User user)
        {

            // * https://octokitnet.readthedocs.io/en/latest/getting-started/

            // ! j'ai besoin du token quand meme, passé en constructeur ?
            _githubClient.Credentials = new Credentials(user.GithubToken);

            var createRepository = new NewRepository(RepositoryName);
            createRepository.Description = Description;
            var repository = await _githubClient.Repository.Create(createRepository);

            // ? add new repo to database ?

            // ? error checks ?
            if (repository == null) {
                throw new Exceptions.ApiException("Failed to call API");
            }

            // * title is required, the rest is optional (check for null values)
            // HttpRequestMessage requestMessage = new HttpRequestMessage();
            // requestMessage.Content = JsonContent.Create(new {
            //     Title,
            //     Body,
            //     Labels,
            //     Assignees
            // });

            // IssueJson ?response = await _httpClient.PostAsync<IssueJson>($"https://api.github.com/repos/{UserName}/{Repository}/issues", requestMessage);
            // if (response == null)
            //     throw new Exceptions.ApiException("Failed to call API");
        }
    }
}
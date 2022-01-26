using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using System.Net.Http.Headers;
using Octokit;

namespace HookHook.Backend.Reactions
{
    public class GithubCreateIssue : IReaction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        public string Title { get; private init; }
        public string Body { get; private init; }
        public string[] Labels { get; private init; }
        public string[] Assignees { get; private init; }

        public GitHubClient _githubClient;
        private readonly HttpClient _httpClient = new();

        public GithubCreateIssue(string user, string repository, string title, string body, string[] labels, string[] assignees)
        {
            UserName = user;
            Repository = repository;
            Title = title;
            Body = body;
            Labels = labels;
            Assignees = assignees;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }

        public async Task Execute(Entities.User user)
        {
            // * https://octokitnet.readthedocs.io/en/latest/getting-started/

            _githubClient.Credentials = new Credentials(user.GithubToken);

            var createIssue = new NewIssue(Title);
            createIssue.Body = Body;
            var issue = await _githubClient.Issue.Create(UserName, Repository, createIssue);

            // ? add new issue to database ?

            // ? error checks ?
            if (issue == null) {
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
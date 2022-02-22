using HookHook.Backend.Utilities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Services;

namespace HookHook.Backend.Actions
{
    [Service("github", "new repository is created")]
    [BsonIgnoreExtraElements]
    public class GithubNewRepository : IAction
    {
        public string UserName {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public List<long> StoredRepositories { get; private init; } = new();

        public GithubNewRepository(string user)
        {
            UserName = user;
            // _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.OAuthAccounts[Providers.GitHub].AccessToken);

            var repositoriesForUser = await _githubClient.Repository.GetAllForUser(UserName);

            foreach (var repository in repositoriesForUser) {
                if (StoredRepositories.Contains(repository.Id))
                    continue;

                StoredRepositories.Add(repository.Id);

                // TODO save in db

                return (repository.Name, true);
            }
            return (null, false);
        }

    }
}
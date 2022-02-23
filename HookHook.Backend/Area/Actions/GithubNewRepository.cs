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

        public string _serviceAccountId { get; set; }

        public GithubNewRepository(string user, string serviceAccountId)
        {
            UserName = user;
            // _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken);

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
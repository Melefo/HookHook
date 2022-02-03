using HookHook.Backend.Utilities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using Octokit;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Actions
{
    [BsonIgnoreExtraElements]
    public class GithubNewRepository : IAction
    {
        public string UserName {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public List<long> StoredRepositories { get; private init; } = new();

        public GithuNewRepository(string user)
        {
            UserName = user;
            // _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.GitHubOAuth.AccessToken);

            var repositoriesForUser = await _githubClient.Repository.GetAllForUser(UserName);

            foreach (var repository in repositoriesForUser) {
                if (StoredRepositories.Contains(repository.Id))
                    continue;

                StoredRepositories.Add(repository.Id);

                return (repository.Name, true);
            }
            return (null, false);
        }

    }
}
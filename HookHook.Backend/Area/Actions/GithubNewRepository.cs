using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Actions
{
    [Service(Providers.GitHub, "new repository is created")]
    [BsonIgnoreExtraElements]
    public class GithubNewRepository : IAction
    {
        public string UserName {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public List<long> StoredRepositories { get; private init; } = new();

        public string AccountId { get; set; }

        public GithubNewRepository(string user, string serviceAccountId, Entities.User userEntity)
        {
            UserName = user;
            AccountId = serviceAccountId;

            // * get repos and store them
            var currentRepositories = GetRepositories(userEntity).GetAwaiter().GetResult();
            foreach (var repo in currentRepositories) {

                Console.WriteLine("Getting existing commits: " + repo.Id);
                StoredRepositories.Add(repo.Id);
            }
        }

        private async Task<IReadOnlyList<Repository>> GetRepositories(Entities.User user)
        {
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var repositoriesForUser = await _githubClient.Repository.GetAllForUser(UserName);

            return (repositoriesForUser);
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            var repositoriesForUser = await GetRepositories(user);

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
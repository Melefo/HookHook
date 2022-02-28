using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// GitHub new repository action
    /// </summary>
    [Service(Providers.GitHub, "new repository is created")]
    [BsonIgnoreExtraElements]
    public class GitHubNewRepository : IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "repo.name", "repo.id", "repo.date", "repo.description", "repo.url"
        };

        /// <summary>
        /// GitHub username
        /// </summary>
        public string Username {get; private init;}
        /// <summary>
        /// GitHub service cccount Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved repositories
        /// </summary>
        public List<long> StoredRepositories { get; private init; } = new();

        /// <summary>
        /// Client used to check on GitHub API
        /// </summary>
        private GitHubClient _githubClient;

        /// <summary>
        /// GitHubNewRepository constructor used by Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public GitHubNewRepository() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        /// <summary>
        /// GitHubNewRepository constructor
        /// </summary>
        /// <param name="username">GitHub Username</param>
        /// <param name="accountId">GitHub service account Id</param>
        /// <param name="user">HookHook user</param>
        public GitHubNewRepository([ParameterName("Username")] string username, string accountId, Entities.User user) : this()
        {
            Username = username;
            AccountId = accountId;

            var currentRepositories = GetRepositories(user).GetAwaiter().GetResult();

            foreach (var repo in currentRepositories)
                StoredRepositories.Add(repo.Id);
        }

        /// <summary>
        /// Get all user repositories
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>A list of repositories</returns>
        private async Task<IReadOnlyList<Repository>> GetRepositories(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var repositoriesForUser = await _githubClient.Search.SearchRepo(new SearchRepositoriesRequest()
            {
                User = Username
            });

            return repositoriesForUser.Items;
        }

        /// <summary>
        /// Check if a new repository is created
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>A lsit of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(Entities.User user)
        {
            var repositoriesForUser = await GetRepositories(user);

            foreach (var repository in repositoriesForUser) {
                if (StoredRepositories.Contains(repository.Id))
                    continue;

                StoredRepositories.Add(repository.Id);
                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], repository.Name },
                    { Formatters[1], repository.Id },
                    { Formatters[2], repository.CreatedAt.ToString("D") },
                    { Formatters[3], repository.Description },
                    { Formatters[4], repository.Url }
                };
                return (formatters, true);
            }
            return (null, false);
        }

    }
}
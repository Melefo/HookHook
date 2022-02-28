using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// GitHub new commit action
    /// </summary>
    [Service(Providers.GitHub, "new commit is done")]
    [BsonIgnoreExtraElements]
    public class GitHubNewCommit : IAction
    {
        /// <summary>
        /// List of formatters of reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "commit.sha", "commit.msg", "author.id", "author.name", "author.login"
        };

        /// <summary>
        /// GitHub username
        /// </summary>
        public string Username {get; private init;}
        /// <summary>
        /// GitHub repository
        /// </summary>
        public string Repository {get; private init;}
        /// <summary>
        /// GitHub service account id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved commits
        /// </summary>
        public List<string> StoredCommitHashes { get; private init; } = new();

        /// <summary>
        /// Client used to check on GitHub API
        /// </summary>
        private readonly GitHubClient _githubClient;

        /// <summary>
        /// GitHubNewCommit constructor used by Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public GitHubNewCommit() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        /// <summary>
        /// GitHubNewCommit constructor
        /// </summary>
        /// <param name="username">GitHub username</param>
        /// <param name="repository">GitHub repository</param>
        /// <param name="accountId">GitHub service account ID</param>
        /// <param name="user">HookHook user</param>
        public GitHubNewCommit([ParameterName("Username")] string username, [ParameterName("Repository name")] string repository, string accountId, Entities.User user) : this()
        {
            Username = username;
            Repository = repository;
            AccountId = accountId;

            var currentRepositoryCommits = GetCommits(user).GetAwaiter().GetResult();

            foreach (var commit in currentRepositoryCommits)
                StoredCommitHashes.Add(commit.Sha);
        }

        /// <summary>
        /// Get a list of all commits
        /// </summary>
        /// <param name="user">HookHook User</param>
        /// <returns>A list of commits</returns>
        private async Task<IReadOnlyList<GitHubCommit>> GetCommits(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var commits = await _githubClient.Repository.Commit.GetAll(Username, Repository);

            return commits;
        }

        /// <summary>
        /// Check if a new commit is pushed
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>List of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(Entities.User user)
        {
            var commits = await GetCommits(user);

            foreach (var commit in commits)
            {
                if (StoredCommitHashes.Contains(commit.Sha))
                    continue;

                StoredCommitHashes.Add(commit.Sha);
                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], commit.Sha },
                    { Formatters[1], commit.Commit.Message },
                    { Formatters[2], commit.Author?.Id },
                    { Formatters[3], commit.Commit.Author.Name },
                    { Formatters[4], commit.Author?.Login }
                };

                return (formatters, true);
            }
            return (null, false);
        }

    }
}
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// GitHub new issue created action
    /// </summary>
    [Service(Providers.GitHub, "new issue is created")]
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("GithubIssueCreated")]
    public class GitHubIssueCreated : IAction
    {
        /// <summary>
        /// List of Formatters used in reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "issue.title", "issue.id", "issue.body", "issue.date", "issue.url", "author.id", "author.name", "author.login"
        };

        /// <summary>
        /// GitHub username
        /// </summary>
        public string Username {get; private init;}
        /// <summary>
        /// GitHub Repository
        /// </summary>
        public string Repository {get; private init;}
        /// <summary>
        /// GitHub service account ID
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of savec issues
        /// </summary>
        public List<int> StoredIssues { get; private init; } = new();

        /// <summary>
        /// Client used to check GitHub API
        /// </summary>
        private readonly GitHubClient _githubClient;

        /// <summary>
        /// GitHubIssueCreated constructor used by Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public GitHubIssueCreated() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        /// <summary>
        /// GitHubIssueCreated construtor
        /// </summary>
        /// <param name="username">GitHub username</param>
        /// <param name="repository">GitHub repository</param>
        /// <param name="accountId">GitHub service account ID</param>
        /// <param name="user">HookHook user</param>
        public GitHubIssueCreated([ParameterName("Username")] string username, [ParameterName("Repository name")] string repository, string accountId, Entities.User user) : this()
        {
            Username = username;
            Repository = repository;
            AccountId = accountId;

            var currentRepositoryIssues = GetIssues(user).GetAwaiter().GetResult();

            foreach (var issue in currentRepositoryIssues)
                StoredIssues.Add(issue.Id);
        }

        /// <summary>
        /// Get a list of all issues
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>a list of issues</returns>
        private async Task<IReadOnlyList<Issue>> GetIssues(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var issuesForRepository = await _githubClient.Issue.GetAllForRepository(Username, Repository);

            return issuesForRepository;
        }

        /// <summary>
        /// Check if a new issue is created
        /// </summary>
        /// <param name="user">HookHook User</param>
        /// <returns>List of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(Entities.User user)
        {
            var issuesForRepository = await GetIssues(user);

            foreach (var issue in issuesForRepository)
            {
                if (StoredIssues.Contains(issue.Id))
                    continue;

                StoredIssues.Add(issue.Id);
                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], issue.Title },
                    { Formatters[1], issue.Id },
                    { Formatters[2], issue.Body },
                    { Formatters[3], issue.CreatedAt.ToString("G") },
                    { Formatters[4], issue.Url },
                    { Formatters[5], issue.User.Id },
                    { Formatters[6], issue.User.Name },
                    { Formatters[7], issue.User.Login }
                };

                return (formatters, true);
            }
            return (null, false);
        }

    }
}
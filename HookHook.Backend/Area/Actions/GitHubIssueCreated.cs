using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.GitHub, "new issue is created")]
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("GithubIssueCreated")]
    public class GitHubIssueCreated : IAction
    {
        public string Username {get; private init;}
        public string Repository {get; private init;}
        public string AccountId { get; set; }

        public List<int> StoredIssues { get; private init; } = new();

        private readonly GitHubClient _githubClient;

        [BsonConstructor]
        public GitHubIssueCreated() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        public GitHubIssueCreated([ParameterName("Username")] string username, [ParameterName("Repository name")] string repository, string accountId, Entities.User user) : this()
        {
            Username = username;
            Repository = repository;
            AccountId = accountId;

            var currentRepositoryIssues = GetIssues(user).GetAwaiter().GetResult();

            foreach (var issue in currentRepositoryIssues)
                StoredIssues.Add(issue.Id);
        }

        private async Task<IReadOnlyList<Issue>> GetIssues(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var issuesForRepository = await _githubClient.Issue.GetAllForRepository(Username, Repository);

            return issuesForRepository;
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            var issuesForRepository = await GetIssues(user);

            foreach (var issue in issuesForRepository)
            {
                if (StoredIssues.Contains(issue.Id))
                    continue;

                StoredIssues.Add(issue.Id);

                return (issue.Title, true);
            }
            return (null, false);
        }

    }
}
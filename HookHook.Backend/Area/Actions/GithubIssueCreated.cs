using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Actions
{
    [Service(Providers.GitHub, "new issue is created")]
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("GithubIssueCreated")]
    public class GithubIssueCreated : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;

        [BsonIgnore]
        public MongoService _db;

        public List<int> StoredIssues { get; private init; } = new();

        public string AccountId { get; set; }

        public GithubIssueCreated(string user, string repository, string accountId, MongoService db, Entities.User userEntity)
        {
            UserName = user;
            Repository = repository;
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));
            AccountId = accountId;

            _db = db;

            // * get issues and store them
            var currentRepositoryIssues = GetIssues(userEntity).GetAwaiter().GetResult();
            foreach (var issue in currentRepositoryIssues) {
                StoredIssues.Add(issue.Id);
            }
        }

        private async Task<IReadOnlyList<Issue>> GetIssues(Entities.User user)
        {
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var issuesForRepository = await _githubClient.Issue.GetAllForRepository(UserName, Repository);

            return (issuesForRepository);
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            var issuesForRepository = await GetIssues(user);

            foreach (var issue in issuesForRepository)
            {
                if (StoredIssues.Contains(issue.Id))
                    continue;

                // await reaction.Execute();
                StoredIssues.Add(issue.Id);

                return (issue.Title, true);
            }
            return (null, false);
        }

    }
}
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;

namespace HookHook.Backend.Area.Actions
{
    public class CommitAuthor
    {
        public string ?Name {get; set;}
        public string ?Date {get; set;}
    }

    public class CommitObject
    {
        public string ?Url {get; set;}
        public string ?Sha {get; set;}
        public CommitAuthor ?Author {get; set;}
        public string ?Message {get; set;}
    }

    public class CommitJson
    {
        public CommitObject ?Commit {get; set;}
    }

    [Service(Providers.GitHub, "new commit is done")]
    [BsonIgnoreExtraElements]
    public class GitHubNewCommit : IAction
    {
        public static string[] Formatters { get; } = new[]
        {
            "commit.sha", "commit.msg", "author.id", "author.name", "author.login"
        };

        public string Username {get; private init;}
        public string Repository {get; private init;}
        public string AccountId { get; set; }

        public List<string> StoredCommitHashes { get; private init; } = new();

        private readonly GitHubClient _githubClient;

        [BsonConstructor]
        public GitHubNewCommit() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        public GitHubNewCommit([ParameterName("Username")] string username, [ParameterName("Repository name")] string repository, string accountId, Entities.User user) : this()
        {
            Username = username;
            Repository = repository;
            AccountId = accountId;

            var currentRepositoryCommits = GetCommits(user).GetAwaiter().GetResult();

            foreach (var commit in currentRepositoryCommits)
                StoredCommitHashes.Add(commit.Sha);
        }

        private async Task<IReadOnlyList<GitHubCommit>> GetCommits(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var commits = await _githubClient.Repository.Commit.GetAll(Username, Repository);

            return commits;
        }

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
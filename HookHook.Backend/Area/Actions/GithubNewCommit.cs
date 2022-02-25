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
    public class GithubNewCommit : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;

        public List<string> StoredCommitHashes { get; private init; } = new();

        public string AccountId { get; set; }

        public GithubNewCommit([ParameterName("Username")] string user, [ParameterName("Repository name")] string repository, string accountId, Entities.User userEntity)
        {
            UserName = user;
            Repository = repository;
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));
            AccountId = accountId;

            // * get commits and store them
            var currentRepositoryCommits = GetCommits(userEntity).GetAwaiter().GetResult();
            foreach (var commit in currentRepositoryCommits) {
                var sha = commit.Commit!.Sha!;

                Console.WriteLine("Getting existing commits: " + sha);
                StoredCommitHashes.Add(sha);
            }
        }

        private async Task<IReadOnlyList<GitHubCommit>> GetCommits(Entities.User user)
        {
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            // * ça marche peut etre uniquement sur master?
            var commits = await _githubClient.Repository.Commit.GetAll(UserName, Repository);

            return (commits);
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            var commits = await GetCommits(user);

            foreach (var commit in commits)
            {
                var sha = commit.Commit!.Sha!;
                if (StoredCommitHashes.Contains(sha))
                    continue;

                // await reaction.Execute();
                StoredCommitHashes.Add(sha);

                return (commit.Commit.Message, true);
            }
            return (null, false);
        }

    }
}
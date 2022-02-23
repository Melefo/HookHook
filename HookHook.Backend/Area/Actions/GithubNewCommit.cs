using HookHook.Backend.Utilities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using Octokit;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;
using HookHook.Backend.Services;

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

    [Service("github", "new commit is done")]
    [BsonIgnoreExtraElements]
    public class GithubNewCommit : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;
        [BsonIgnore]
        private HttpClient _httpClient = new();

        public List<string> StoredCommitHashes { get; private init; } = new();

        public string _serviceAccountId;

        public GithubNewCommit(string user, string repository, string serviceAccountId)
        {
            UserName = user;
            Repository = repository;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken);

            var commits = await _githubClient.Repository.Commit.GetAll(UserName, Repository);

            foreach (var commit in commits)
            {
                if (StoredCommitHashes.Contains(commit.Commit.Sha))
                    continue;

                // await reaction.Execute();
                StoredCommitHashes.Add(commit.Commit.Sha);

                return (commit.Commit.Message, true);
            }
            return (null, false);
        }

    }
}
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
        private readonly HttpClient _httpClient = new();

        public List<string> StoredCommitHashes { get; private init; } = new();

        public GithubNewCommit(string user, string repository)
        {
            UserName = user;
            Repository = repository;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {user.OAuthAccounts[Providers.GitHub].AccessToken}");

            CommitJson[] ?response = await _httpClient.GetAsync<CommitJson[]>($"https://api.github.com/repos/{UserName}/{Repository}/commits");
            if (response == null)
                throw new Exceptions.ApiException("Failed to call API");

            foreach (var commit in response)
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
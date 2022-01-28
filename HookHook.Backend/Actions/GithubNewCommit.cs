using HookHook.Backend.Utilities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using Octokit;

namespace HookHook.Backend.Actions
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

    public class GithubNewCommit : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        public GitHubClient _githubClient;
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
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {user.GitHub.AccessToken}");

            CommitJson[] ?response = await _httpClient.GetAsync<CommitJson[]>($"https://api.github.com/repos/{UserName}/{Repository}/commits");
            if (response == null)
                throw new Exceptions.ApiException("Failed to call API");

            foreach (var commit in response)
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
using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using System.Net.Http.Headers;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Actions
{
    [Service("github", "new issue is created")]
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("GithubIssueCreated")]
    public class GithubIssueCreated : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;
        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public List<int> StoredIssues { get; private init; } = new();

        public string _serviceAccountId {get; private init;}

        public GithubIssueCreated(string user, string repository, string serviceAccountId)
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

            var issuesForRepository = await _githubClient.Issue.GetAllForRepository(UserName, Repository);

            foreach (var issue in issuesForRepository)
            {
                if (StoredIssues.Contains(issue.Id))
                    continue;

                // await reaction.Execute();
                StoredIssues.Add(issue.Id);
                Console.WriteLine("Found a new issue");

                // todo save your storedIssues

                return (issue.Title, true);
            }
            return (null, false);
        }

    }
}
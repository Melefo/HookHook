using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using System.Net.Http.Headers;
using Octokit;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Actions
{
    [BsonIgnoreExtraElements]
    public class GithubIssueCreated : IAction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;
        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public List<int> StoredIssues { get; private init; } = new();

        public GithubIssueCreated(string user, string repository)
        {
            UserName = user;
            Repository = repository;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _githubClient.Credentials = new Credentials(user.GitHubOAuth.AccessToken);

            var issuesForRepository = await _githubClient.Issue.GetAllForRepository(UserName, Repository);

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
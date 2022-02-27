using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new issue")]
    [BsonDiscriminator("GithubCreateIssue")]
    public class GithubCreateIssue : IReaction
    {
        public string Username {get; private init;}
        public string Repository {get; private init;}
        public string Title { get; private init; }
        public string Body { get; private init; }
        public string AccountId { get; set; }

        private readonly GitHubClient _githubClient;

        [BsonConstructor]
        public GithubCreateIssue() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        public GithubCreateIssue([ParameterName("Username")] string username, [ParameterName("Repository")] string repository, [ParameterName("Issue title")] string title, [ParameterName("Issue body")] string body, string accountId) : this()
        {
            Username = username;
            Repository = repository;
            Title = title;
            Body = body;
            AccountId = accountId;
        }

        public async Task Execute(Entities.User user, string actionInfo)
        {
            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var createIssue = new NewIssue(Title);
            createIssue.Body = Body;
            var issue = await _githubClient.Issue.Create(Username, Repository, createIssue);

            if (issue == null)
                throw new Exceptions.ApiException("Failed to call API");
        }
    }
}
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new issue")]
    [BsonDiscriminator("GithubCreateIssue")]
    public class GithubCreateIssue : IReaction
    {
        public string UserName {get; private init;}
        public string Repository {get; private init;}

        public string Title { get; private init; }
        public string Body { get; private init; }
        public List<string> Labels { get; private init; } = new();
        public List<string> Assignees { get; private init; } = new();

        [BsonIgnore]
        public GitHubClient _githubClient;

        public string AccountId { get; set; }

        public GithubCreateIssue(string user, string repository, string title, string body, string serviceAccountId)
        {
            UserName = user;
            Repository = repository;
            Title = title;
            Body = body;
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));
            AccountId = serviceAccountId;
        }

        public async Task Execute(Entities.User user, string actionInfo)
        {
            // * https://octokitnet.readthedocs.io/en/latest/getting-started/
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var createIssue = new NewIssue(Title);
            createIssue.Body = Body;
            var issue = await _githubClient.Issue.Create(UserName, Repository, createIssue);

            // ? add new issue to database ?

            if (issue == null) {
                throw new Exceptions.ApiException("Failed to call API");
            }
        }
    }
}
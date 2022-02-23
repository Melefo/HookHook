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
        public string[] Labels { get; private init; }
        public string[] Assignees { get; private init; }

        [BsonIgnore]
        public GitHubClient _githubClient;

        public string _serviceAccountId { get; set; }

        public GithubCreateIssue(string user, string repository, string title, string body, string serviceAccountId)
        {
            UserName = user;
            Repository = repository;
            Title = title;
            Body = body;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
            _serviceAccountId = serviceAccountId;
        }

        public async Task Execute(Entities.User user)
        {
            // * https://octokitnet.readthedocs.io/en/latest/getting-started/
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken);

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
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Reactions
{
    /// <summary>
    /// Create a GitHub issue reaction
    /// </summary>
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new issue")]
    [BsonDiscriminator("GithubCreateIssue")]
    public class GithubCreateIssue : IReaction
    {
        /// <summary>
        /// GitHub username
        /// </summary>
        public string Username {get; private init;}
        /// <summary>
        /// GitHub repository
        /// </summary>
        public string Repository {get; private init;}
        /// <summary>
        /// Issue title
        /// </summary>
        public string Title { get; private init; }
        /// <summary>
        /// Issue body
        /// </summary>
        public string Body { get; private init; }
        /// <summary>
        /// GitHub service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Client used to check of GitHub API
        /// </summary>
        private readonly GitHubClient _githubClient;

        /// <summary>
        /// GithubCreateIssue constructor for Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public GithubCreateIssue() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        /// <summary>
        /// GithubCreateIssue constructor
        /// </summary>
        /// <param name="username">GitHub username</param>
        /// <param name="repository">GitHub repository</param>
        /// <param name="title">Issue title</param>
        /// <param name="body">Issue body</param>
        /// <param name="accountId">GitHub service account Id</param>
        public GithubCreateIssue([ParameterName("Username")] string username, [ParameterName("Repository")] string repository, [ParameterName("Issue title")] string title, [ParameterName("Issue body")] string body, string accountId) : this()
        {
            Username = username;
            Repository = repository;
            Title = title;
            Body = body;
            AccountId = accountId;
        }

        /// <summary>
        /// Create a new issue
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">List of formatters from action</param>
        /// <returns></returns>
        /// <exception cref="Exceptions.ApiException"></exception>
        public async Task Execute(Entities.User user, Dictionary<string, object?> formatters)
        {
            var username = Username.FormatParam(formatters);
            var repository = Repository.FormatParam(formatters);
            var title = Title.FormatParam(formatters);
            var body = Body.FormatParam(formatters);

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var createIssue = new NewIssue(title);
            createIssue.Body = body;
            var issue = await _githubClient.Issue.Create(username, repository, createIssue);

            if (issue == null)
                throw new Exceptions.ApiException("Failed to call API");
        }
    }
}
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Reactions
{
    /// <summary>
    /// Create a GitHub repository reaction
    /// </summary>
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new repository")]
    public class GithubCreateRepository : IReaction
    {
        /// <summary>
        /// GitHub repository name
        /// </summary>
        public string RepositoryName {get; private init;}
        /// <summary>
        /// GitHub repository description
        /// </summary>
        public string Description {get; private init;}
        /// <summary>
        /// GitHub service accoutn id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Client used to check on GitHub API
        /// </summary>
        private readonly GitHubClient _githubClient;

        /// <summary>
        /// GithubCreateRepository constructor for Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public GithubCreateRepository() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        /// <summary>
        /// GithubCreateRepository constructor
        /// </summary>
        /// <param name="repositoryName">GitHub repository name</param>
        /// <param name="description">GitHub repository description</param>
        /// <param name="accountId">GitHub service account Id</param>
        public GithubCreateRepository([ParameterName("Repository name")] string repositoryName, [ParameterName("Repository description")] string description, string accountId) : this()
        {
            RepositoryName = repositoryName;
            Description = description;
            AccountId = accountId;
        }

        /// <summary>
        /// Create Discord repository
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">List of formatters from reactions</param>
        /// <returns></returns>
        /// <exception cref="Exceptions.ApiException"></exception>
        public async Task Execute(Entities.User user, Dictionary<string, object?> formatters)
        {
            var name = RepositoryName.FormatParam(formatters);
            var description = Description.FormatParam(formatters);

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var createRepository = new NewRepository(name);
            createRepository.Description = description;
            var repository = await _githubClient.Repository.Create(createRepository);

            if (repository == null)
                throw new Exceptions.ApiException("Failed to call API");
        }
    }
}
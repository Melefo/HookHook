using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new repository")]
    public class GithubCreateRepository : IReaction
    {
        public string RepositoryName {get; private init;}
        public string Description {get; private init;}
        public string AccountId { get; set; }

        private readonly GitHubClient _githubClient;

        [BsonConstructor]
        public GithubCreateRepository() =>
            _githubClient = new GitHubClient(new ProductHeaderValue("HookHook"));

        public GithubCreateRepository([ParameterName("Repository name")] string repositoryName, [ParameterName("Repository description")] string description, string accountId) : this()
        {
            RepositoryName = repositoryName;
            Description = description;
            AccountId = accountId;
        }

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
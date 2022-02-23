using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.GitHub, "create a new repository")]
    public class GithubCreateRepository : IReaction
    {
        public string RepositoryName {get; private init;}

        public string Description {get; private init;}

        [BsonIgnore]
        public GitHubClient _githubClient;
        [BsonIgnore]
        private readonly HttpClient _httpClient = new();

        public string _serviceAccountId { get; set; }

        public GithubCreateRepository(string repositoryName, string description, string serviceAccountId)
        {
            RepositoryName = repositoryName;
            Description = description;
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));
            _serviceAccountId = serviceAccountId;
        }

        public async Task Execute(Entities.User user)
        {
            _githubClient = new GitHubClient(new Octokit.ProductHeaderValue("HookHook"));

            // * https://octokitnet.readthedocs.io/en/latest/getting-started/

            _githubClient.Credentials = new Credentials(user.ServicesAccounts[Providers.GitHub].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken);

            var createRepository = new NewRepository(RepositoryName);
            createRepository.Description = Description;
            var repository = await _githubClient.Repository.Create(createRepository);

            // ? add new repo to database ?

            // ? error checks ?
            if (repository == null) {
                throw new Exceptions.ApiException("Failed to call API");
            }
        }
    }
}
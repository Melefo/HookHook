using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using System.Net.Http.Headers;

namespace HookHook.Backend.Services
{

    public class Label
    {
        public int Id {get; set;}
        public string ?Name {get; set;}
        public string ?Description {get; set;}
        public string ?Color {get; set;}
    }

    public class GithubUser
    {
        public string ?Login {get; set;}
        public int Id {get; set;}
        public string ?Avatar_Url {get; set;}
        public string ?Html_Url {get; set;}
    }

    public class IssueJson
    {
        public int Id {get; set;}
        public string ?Url {get; set;}
        public string ?Repository_Url {get; set;}
        public int Number {get; set;}
        public string ?State {get; set;}
        public string ?Title {get; set;}
        public string ?Body {get; set;}
        public GithubUser ?User {get; set;}
        public GithubUser[] ?Assignees {get; set;}
        public string ?Created_At {get; set;}
        public string ?Updated_At {get; set;}
    }

    public class CommitAuthor
    {
        public string ?Name {get; set;}
        public string ?Date {get; set;}
    }

    public class CommitObject
    {
        public string ?Url {get; set;}
        public CommitAuthor ?Author {get; set;}
        public string ?Message {get; set;}
    }

    public class CommitJson
    {
        public CommitObject ?Commit {get; set;}
    }

    public class Repository
    {
        public string ?Name {get; set;}
        public GithubUser ?Owner {get; set;}
        public bool Private {get; set;}
        public string ?Description {get; set;}
    }

    public class GithubService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client = new();

        /// <summary>
        /// Github Service Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="mongo"></param>
        public GithubService(IConfiguration config, MongoService mongo)
        {
            _apiKey = config["Github:ApiKey"];
            _client.DefaultRequestHeaders.Clear();

            _client.DefaultRequestHeaders.Add("Authorization", $"token {_apiKey}");
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd("HookHook");
            // _client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        }

        public async Task<RepositoryData> CreateRepository(RepositoryModel repoModel)
        {
            // * fetch user id (with jwt ?)
            // * fetch area of user with areaID
            // * cross check attatched user with connected user

            // * change the authorization token to github oauth token from database

            HttpRequestMessage requestMessage = new HttpRequestMessage();

            // * name is required, the rest is optional (check for null values)
            requestMessage.Content = JsonContent.Create(new {
                name = repoModel.RepositoryName,
                description = repoModel.Description,
                @private = repoModel.Private
            });

            Repository ?response = await _client.PostAsync<Repository>($"https://api.github.com/user/repos", requestMessage);
            if (response == null)
                throw new ApiException("Failed to call API");

            return (new RepositoryData(response.Name, response.Description, response.Owner.Login, response.Private));
        }

        // * getLatestRepository ('')
        public async Task<RepositoryData> GetLatestRepository(string areaID)
        {
            // * fetch user id (with jwt ?)
            // * fetch area of user with areaID
            // * cross check attatched user with connected user

            // * change the authorization token to github oauth token from database

            // * githubUserName = area.action.user
            // * githubRepoName = area.action.repository

            string userName = "niklasf";

            Repository[] ?response = await _client.GetAsync<Repository[]>($"https://api.github.com/users/{userName}/repos");
            if (response == null)
                throw new ApiException("Failed to call API");

            RepositoryData repoData = new RepositoryData(response[0].Name, response[0].Description, response[0].Owner.Login, response[0].Private);
            return (repoData);
        }

        // * getRepositoriesFromUser (pour faire un dropdown éventuellement ?)
    }
}
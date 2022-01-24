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

        // * createIssue (oauth)
        // * createRepository (oauth)

        // * getLatestIssue (needs oauth for private repos)
        public async Task<IssueData> GetLatestIssue(string areaID)
        {
            // * fetch user id (with jwt ?)
            // * fetch area of user with areaID
            // * cross check attatched user with connected user

            // * githubUserName = area.action.user
            // * githubRepoName = area.action.repository
            string userName = "The-Law-1";
            string repoName = "Redditech";

            IssueJson[] ?response = await _client.GetAsync<IssueJson[]>($"https://api.github.com/repos/{userName}/{repoName}/issues");
            if (response == null)
                throw new ApiException("Failed to call API");

            return (new IssueData(response[0].Title, response[0].Body, response[0].User.Html_Url));
        }

        // * getLatestCommit ('')
        // * getLatestRepository ('')

        // * getRepositoriesFromUser (pour faire un dropdown éventuellement ?)
    }
}
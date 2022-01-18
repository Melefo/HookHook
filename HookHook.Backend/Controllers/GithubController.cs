using Microsoft.AspNetCore.Mvc;
using HookHook.Backend.Models;
using HookHook.Backend.Models.Github;

namespace HookHook.Backend.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class GithubController : ControllerBase
	{
		private string _githubAPIKey;

		public GithubController(IConfiguration configuration)
		{
			_githubAPIKey = configuration["Github:APIKey"];
		}

		// * create commit, needs oauth + authorization
		[HttpPost("CreateIssue")]
		public IssueData CreateIssue(IssueModel issueModel)
		{
			// * call the service

			return (new IssueData { Name = issueModel.Name });
		}

		// * create repo, needs oauth
		[HttpPost("CreateRepository")]
		public RepositoryData CreateRepository(RepositoryModel repoModel)
		{
			// * call the service

			return (new RepositoryData { RepositoryName = repoModel.RepositoryName });
		}

		// * get latest issue, for private repos needs oauth
		[HttpGet("GetLatestIssue")]
		public IssueData GetLatestIssue([FromBody] RepositoryModel repoModel)
		{

			return (new IssueData { Name = "New issue" });
		}

		// * get latest commit, for private repos needs oauth
		[HttpGet("GetLatestCommit")]
		public CommitData GetLatestCommit([FromBody] RepositoryModel repository)
		{
			// * call the service which cross-checks in database
			return (new CommitData { Message = "fix bug" });
		}

		// * either takes a username for public repos, or uses the personal oauth token for private repos
		[HttpGet("GetLatestRepository")]
		public RepositoryData GetLatestRepository([FromBody] string userName)
		{
			// * call the service which cross-checks in database
			return (new RepositoryData { RepositoryName = "YEP" });
		}
	}
}

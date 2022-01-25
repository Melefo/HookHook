using Microsoft.AspNetCore.Mvc;
using HookHook.Backend.Models;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Services;
using HookHook.Backend.Exceptions;

namespace HookHook.Backend.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class GithubController : ControllerBase
	{

        private readonly GithubService _service;

		public GithubController(GithubService service) =>
            _service = service;

		// * create issue, needs oauth + authorization
		[HttpPost("CreateIssue")]
		public async Task<ActionResult<IssueData>> CreateIssue(IssueModel issueModel)
		{
            try {
                return (await _service.CreateIssue(issueModel));
            } catch (MongoException ex) {
                return BadRequest(new { error = ex.Message });
            } catch (ApiException ex) {
                return (StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message}));
            }
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
		public async Task<ActionResult<IssueData>> GetLatestIssue()
		{
            try {
                return (await _service.GetLatestIssue("1"));
            } catch (MongoException ex) {
                return BadRequest(new { error = ex.Message });
            } catch (ApiException ex) {
                return (StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message}));
            }
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

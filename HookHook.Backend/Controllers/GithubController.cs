using Microsoft.AspNetCore.Mvc;

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

		[HttpGet(Name = "GetLatestRepository")]
		public Github Get()
		{
			// * format data in model
			string rawData = "NewRepo";
			return (new Github { NewRepoName = rawData });
		}
	}
}

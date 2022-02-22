using System;
namespace HookHook.Backend.Services
{
	public class GitHubService
	{
		private readonly string _id;
		private readonly string _secret;

		public GitHubService(IConfiguration config)
		{
			_id = config["Google:ClientId"];
			_secret = config["Google:ClientSecret"];
		}

	}
}


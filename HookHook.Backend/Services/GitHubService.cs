using System;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;
using Octokit;

namespace HookHook.Backend.Services
{
	public class GitHubService
	{
		private readonly string _id;
		private readonly string _secret;

		public GitHubService(IConfiguration config)
		{
			_id = config["GitHub:ClientId"];
			_secret = config["GitHub:ClientSecret"];
		}

		public async Task<(GitHubClient, OauthToken)> OAuth(string code)
        {
			var client = new GitHubClient(new ProductHeaderValue("HookHook"));

			var request = new OauthTokenRequest(_id, _secret, code);
			var res = await client.Oauth.CreateAccessToken(request);

			if (res == null)
				throw new Exceptions.ApiException("Failed to call API");

			client.Credentials = new Credentials(res.AccessToken);

			return (client, res);
		}

		public async Task AddAccount(Entities.User user, string code)
        {
			(var client, var res) = await OAuth(code);
			var current = await client.User.Current();
			var id = current.Id.ToString();

			user.ServicesAccounts.TryAdd(Providers.GitHub, new());
			if (user.ServicesAccounts[Providers.GitHub].Any(x => x.UserId == id))
				return;


			user.ServicesAccounts[Providers.GitHub].Add(new(id.ToString(), res.AccessToken));
        }
	}
}


using System;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Utilities;
using Octokit;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// Service used by areaservice
    /// </summary>
	public class GitHubService
	{
        /// <summary>
        /// Client ID Web
        /// </summary>
		private readonly string _idWeb;
        /// <summary>
        /// Client secret Web
        /// </summary>
		private readonly string _secretWeb;
        /// <summary>
        /// Client ID Mobile
        /// </summary>
        private readonly string _idMobile;
        /// <summary>
        /// Client secret Mobile
        /// </summary>
		private readonly string _secretMobile;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Environment variables</param>
		public GitHubService(IConfiguration config)
		{
			_idWeb = config["GitHub:Web:ClientId"];
			_secretWeb = config["GitHub:Web:ClientSecret"];
            _idMobile = config["GitHub:Mobile:ClientId"];
            _secretMobile = config["GitHub:Mobile:ClientSecret"];
        }

        /// <summary>
        /// OAuth
        /// </summary>
        /// <param name="code">OAuth code</param>
        /// <returns>GithubClient, OauthToken</returns>
		public async Task<(GitHubClient, OauthToken)> OAuth(string code)
        {
			var client = new GitHubClient(new ProductHeaderValue("HookHook"));

            var request = new OauthTokenRequest(_idWeb, _secretWeb, code);
            var res = await client.Oauth.CreateAccessToken(request);

            if (res.AccessToken == null)
            {
                request = new OauthTokenRequest(_idMobile, _secretMobile, code);
                res = await client.Oauth.CreateAccessToken(request);
            }

			if (res == null)
				throw new Exceptions.ApiException("Failed to call API");

			client.Credentials = new Credentials(res.AccessToken);

			return (client, res);
		}

        /// <summary>
        /// Add new service account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code">OAuth code</param>
        /// <returns>New ServiceAccount</returns>
		public async Task<ServiceAccount?> AddAccount(Entities.User user, string code)
        {
			(var client, var res) = await OAuth(code);
			var current = await client.User.Current();
			var id = current.Id.ToString();

			user.ServicesAccounts.TryAdd(Providers.GitHub, new());
			if (user.ServicesAccounts[Providers.GitHub].Any(x => x.UserId == id))
				return null;

			user.ServicesAccounts[Providers.GitHub].Add(new(id.ToString(), res.AccessToken));
			return new(id, current.Login);
        }
	}
}


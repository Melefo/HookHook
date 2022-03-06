using System;
using CoreTweet;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// Service used by areaservice
    /// </summary>
	public class TwitterService
	{
        /// <summary>
        /// IConfiguration configuration
        /// </summary>
		private readonly IConfiguration _configuration;

        /// <summary>
        /// Client ID
        /// </summary>
		private readonly string _id;

        /// <summary>
        /// Client secret
        /// </summary>
		private readonly string _secret;

        /// <summary>
        /// _OAuthSessions
        /// </summary>
		private Dictionary<string, OAuth.OAuthSession> _OAuthSessions = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Environment variables</param>
		public TwitterService(IConfiguration configuration)
		{
			_configuration = configuration;

			_id = _configuration["Twitter:ClientId"];
			_secret = _configuration["Twitter:ClientSecret"];
		}

        /// <summary>
        /// Twitter authorize
        /// </summary>
        /// <returns>Authorization code</returns>
		public string Authorize(string redirect)
		{
			var session = CoreTweet.OAuth.Authorize(_id, _secret, redirect);

			_OAuthSessions.Add(session.RequestToken, session);
			return session.AuthorizeUri.ToString();
		}

        /// <summary>
        /// OAuth
        /// </summary>
        /// <param name="code">OAuth code</param>
        /// <returns>UserResponse, Tokens</returns>
		public async Task<(UserResponse, Tokens)> OAuth(string code, string verifier)
		{
			if (!_OAuthSessions.TryGetValue(code, out var session))
				throw new ApiException("Failed to call API");
			var tokens = await CoreTweet.OAuth.GetTokensAsync(session, verifier);
			_OAuthSessions.Remove(code);

			if (tokens == null)
				throw new ApiException("Failed to call API");

			var twitter = await tokens.Users.ShowAsync(tokens.UserId);

			return (twitter, tokens);
		}

        /// <summary>
        /// Add new service account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <param name="verifier"></param>
        /// <returns>New ServiceAccount</returns>
		public async Task<ServiceAccount?> AddAccount(Entities.User user, string code, string verifier)
        {
			(UserResponse twitter, Tokens tokens) = await OAuth(code, verifier);
			var id = twitter.Id.ToString()!;

			user.ServicesAccounts.TryAdd(Providers.Twitter, new());
			if (user.ServicesAccounts[Providers.Twitter].Any(x => x.UserId == id))
				return null;

			user.ServicesAccounts[Providers.Twitter].Add(new(id, tokens.AccessToken, secret: tokens.AccessTokenSecret));
			var current = await tokens.Users.ShowAsync(twitter.Id!.Value);
			return new(id, $"@{current.ScreenName}");
		}
	}
}


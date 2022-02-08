using System;
using CoreTweet;

namespace HookHook.Backend.Services
{
	public class TwitterService
	{
		private readonly IConfiguration _configuration;

		private readonly string _id;
		private readonly string _secret;
		private readonly string _redirect;

		private Dictionary<string, CoreTweet.OAuth.OAuthSession> _OAuthSessions = new();

		public TwitterService(IConfiguration configuration)
		{
			_configuration = configuration;

			_id = _configuration["Twitter:ClientId"];
			_secret = _configuration["Twitter:ClientSecret"];
			_redirect = _configuration["Twitter:Redirect"];
		}

		public string Authorize()
		{
			var session = CoreTweet.OAuth.Authorize(_id, _secret, _redirect);

			_OAuthSessions.Add(session.RequestToken, session);
			return session.AuthorizeUri.ToString();
		}

		public async Task<Tokens?> Token(string code, string verifier)
		{
			if (!_OAuthSessions.TryGetValue(code, out var session))
				return null;
			var tokens = await CoreTweet.OAuth.GetTokensAsync(session, verifier);
			_OAuthSessions.Remove(code);
			return tokens;
		}
    }
}


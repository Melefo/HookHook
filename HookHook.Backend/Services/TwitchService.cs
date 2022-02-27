using System;
using System.Text.Json.Serialization;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Utilities;
using TwitchLib.Api;

namespace HookHook.Backend.Services
{
    public class TwitchToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string[] Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        public TwitchToken(string accessToken, int expiresIn, string refreshToken, string[] scope, string tokenType)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            Scope = scope;
            TokenType = tokenType;
        }
    }

    public class TwitchService
	{
		private readonly string _id;
		private readonly string _secret;
		private readonly string _redirect;

        private readonly HttpClient _client = new();


        public TwitchService(IConfiguration config)
		{
			_id = config["Twitch:ClientId"];
			_secret = config["Twitch:ClientSecret"];
			_redirect = config["Twitch:Redirect"];
        }

		public async Task<(TwitchLib.Api.Helix.Models.Users.GetUsers.User, TwitchToken)> OAuth(string code)
        {
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _id),
                new("client_secret", _secret),
                new("code", code),
                new("grant_type", "authorization_code"),
                new("redirect_uri", _redirect),
            });
            var res = await _client.PostAsync<TwitchToken>("https://id.twitch.tv/oauth2/token", content);

            if (res == null)
                throw new ApiException("Failed to call API");

            var api = new TwitchAPI();
            api.Settings.ClientId = _id;
            api.Settings.AccessToken = res.AccessToken;

            var users = await api.Helix.Users.GetUsersAsync(null, null, res.AccessToken);

            if (users == null)
                throw new ApiException("Failed to call API");

            var client = users.Users[0];

            return (client, res);
        }

        public async Task<ServiceAccount?> AddAccount(User user, string code)
        {
            (var client, TwitchToken res) = await OAuth(code);
            var id = client.Id;

            user.ServicesAccounts.TryAdd(Providers.Twitch, new());
            if (user.ServicesAccounts[Providers.Twitch].Any(x => x.UserId == id))
                return null;

            user.ServicesAccounts[Providers.Twitch].Add(new(client.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken));
            return new(id, client.Login);
        }

		public async Task Refresh(OAuthAccount account)
        {
            if (account.ExpiresIn == null || account.ExpiresIn > DateTime.UtcNow)
                return;
            if (account.RefreshToken == null)
                return;
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _id),
                new("client_secret", _secret),
                new("grant_type", "refresh_token"),
                new("refresh_token", account.RefreshToken),
            });
            var res = await _client.PostAsync<TwitchToken>("https://id.twitch.tv/oauth2/token", content);
            account.AccessToken = res!.AccessToken;
            account.ExpiresIn = DateTime.UtcNow.Add(TimeSpan.FromSeconds(res.ExpiresIn));
            account.RefreshToken = res.RefreshToken;
        }
	}
}


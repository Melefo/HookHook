using System.Text.Json.Serialization;
using Discord.Rest;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class DiscordToken
    {
        /// <summary>
        /// Discord access token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Access token expiration
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Scope, i.e. rights
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        public DiscordToken(string accessToken, int expiresIn, string refreshToken, string scope, string tokenType)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            Scope = scope;
            TokenType = tokenType;
        }
    }

    /// <summary>
    /// Service used by areaservice
    /// </summary>
    public class DiscordService
	{
        /// <summary>
        /// Client ID
        /// </summary>
        private readonly string _id;
        /// <summary>
        /// Client secret
        /// </summary>
		private readonly string _secret;

        /// <summary>
        /// HTTP client
        /// </summary>
        private readonly HttpClient _client = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Environment variables</param>
        public DiscordService(IConfiguration config)
		{
            _id = config["Discord:ClientId"];
			_secret = config["Discord:ClientSecret"];
		}

        /// <summary>
        /// OAuth
        /// </summary>
        /// <param name="code">OAuth code</param>
        /// <returns>DiscordRestClient, DiscordToken</returns>
        public async Task<(DiscordRestClient, DiscordToken)> OAuth(string code, string? verifier, string redirect)
        {
            var col = new List<KeyValuePair<string, string>>
            {
                new("client_id", _id),
                new("client_secret", _secret),
                new("grant_type", "authorization_code"),
                new("code", code),
                new("redirect_uri", redirect),
                new("scope", "identify guilds email bot")
            };
            if (verifier != null)
                col.Add(new("code_verifier", verifier));

            var content = new FormUrlEncodedContent(col);
            var res = await _client.PostAsync<DiscordToken>("https://discord.com/api/oauth2/token", content);

            if (res == null)
                throw new ApiException("Failed to call API");
            var client = new DiscordRestClient();
            await client.LoginAsync(Discord.TokenType.Bearer, res.AccessToken);

            return (client, res);
        }

        /// <summary>
        /// Add new service account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns>New ServiceAccount</returns>
        public async Task<ServiceAccount?> AddAccount(User user, string code, string? verifier, string redirect)
        {
            (DiscordRestClient client, DiscordToken token) = await OAuth(code, verifier, redirect);
            var id = client.CurrentUser.Id.ToString();

            user.ServicesAccounts.TryAdd(Providers.Discord, new());
            if (user.ServicesAccounts[Providers.Discord].Any(x => x.UserId == id))
                return null;

            user.ServicesAccounts[Providers.Discord].Add(new(id, token.AccessToken, TimeSpan.FromSeconds(token.ExpiresIn), token.RefreshToken));
            return new(id, client.CurrentUser.ToString());
        }

        /// <summary>
        /// Refresh discord account tokens
        /// </summary>
        /// <param name="account"></param>
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

            var res = await _client.PostAsync<DiscordToken>("https://discord.com/api/oauth2/token", content);
            account.AccessToken = res!.AccessToken;
            account.ExpiresIn = DateTime.UtcNow.Add(TimeSpan.FromSeconds(res.ExpiresIn));
            account.RefreshToken = res.RefreshToken;
        }
    }
}


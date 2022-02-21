﻿using System;
using System.Text.Json.Serialization;
using Discord.Rest;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Services
{
    public class DiscordToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    public class DiscordService
	{
        private readonly MongoService _db;

        private readonly string _discordId;
		private readonly string _discordSecret;
		private readonly string _discordRedirect;

        private readonly HttpClient _client = new();

        public DiscordService(MongoService db, IConfiguration config)
		{
            _db = db;

            _discordId = config["Discord:ClientId"];
			_discordSecret = config["Discord:ClientSecret"];
			_discordRedirect = config["Discord:Redirect"];
		}

        public async Task<(DiscordRestClient, DiscordToken)> OAuth(string code)
        {
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _discordId),
                new("client_secret", _discordSecret),
                new("grant_type", "authorization_code"),
                new("code", code),
                new("redirect_uri", _discordRedirect),
            });
            var res = await _client.PostAsync<DiscordToken>("https://discord.com/api/oauth2/token", content);

            if (res == null)
                throw new ApiException("Failed to call API");
            var client = new DiscordRestClient();
            await client.LoginAsync(Discord.TokenType.Bearer, res.AccessToken);

            return (client, res);
        }

        public async Task AddAccount(User user, string code)
        {
            (DiscordRestClient client, DiscordToken token) = await OAuth(code);
            var id = client.CurrentUser.Id.ToString();

            user.DiscordServices ??= new();
            if (user.DiscordServices.Any(x => x.UserId == id))
                return;

            user.DiscordServices.Add(new(id, token.AccessToken, TimeSpan.FromSeconds(token.ExpiresIn), token.RefreshToken));
            _db.SaveUser(user);
        }

        public async Task Refresh(OAuthAccount account)
        {
            if (account.ExpiresIn == null || account.ExpiresIn > DateTime.UtcNow)
                return;
            if (account.RefreshToken == null)
                return;
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _discordId),
                new("client_secret", _discordSecret),
                new("grant_type", "refresh_token"),
                new("refresh_token", account.RefreshToken),
            });

            var res = await _client.PostAsync<DiscordToken>("https://discord.com/api/oauth2/token", content);
            account.AccessToken = res.AccessToken;
            account.ExpiresIn = DateTime.UtcNow.Add(TimeSpan.FromSeconds(res.ExpiresIn));
            account.RefreshToken = res.RefreshToken;
        }
    }
}


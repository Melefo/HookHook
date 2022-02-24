using System;
using FluentScheduler;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Services
{
	public class AreaService : Registry
	{
        private readonly MongoService _mongo;
        private readonly DiscordService _discord;
        private readonly GoogleService _google;
        private readonly SpotifyService _spotify;
        private readonly TwitchService _twitch;

        public AreaService(MongoService mongo, DiscordService discord, GoogleService google, SpotifyService spotify, TwitchService twitch)
        {
            _mongo = mongo;
            _discord = discord;
            _google = google;
            _spotify = spotify;
            _twitch = twitch;

            Schedule(async () => await Execute()).ToRunEvery(1).Minutes();
        }

        public async Task RefreshTokenBeforeExecute(User user)
        {

            if (user.ServicesAccounts.TryGetValue(Providers.Discord, out var discord))
                foreach (var account in discord)
                    await _discord.Refresh(account);
            if (user.ServicesAccounts.TryGetValue(Providers.Google, out var google))
                foreach (var account in google)
                    await _google.Refresh(account);
            if (user.ServicesAccounts.TryGetValue(Providers.Spotify, out var spotify))
                foreach (var account in spotify)
                    await _spotify.Refresh(account);
            if (user.ServicesAccounts.TryGetValue(Providers.Twitch, out var twitch))
                foreach (var account in twitch)
                    await _twitch.Refresh(account);
        }

        public async Task<bool> ExecuteUserArea(User user, string id)
        {
            var area = user.Areas.SingleOrDefault(x => x.Id == id);
            if (area == null)
                return false;

            await RefreshTokenBeforeExecute(user);
            await area.Launch(user, _mongo);
            _mongo.SaveUser(user);
            return true;
        }

        public async Task ExecuteUser(User user)
        {
            await RefreshTokenBeforeExecute(user);

            foreach (var area in user.Areas)
                await area.Launch(user, _mongo);

            _mongo.SaveUser(user);
        }

        private async Task Execute()
        {
            var users = _mongo.GetUsers();

            foreach (var user in users.Where(x => x.Areas.Count > 0))
                await ExecuteUser(user);
        }
	}
}
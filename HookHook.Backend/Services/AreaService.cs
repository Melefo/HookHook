using FluentScheduler;
using HookHook.Backend.Controllers;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// Service used by handle area
    /// </summary>
	public class AreaService : Registry
	{
        /// <summary>
        /// Database access
        /// </summary>
        private readonly MongoService _mongo;
        /// <summary>
        /// Discord service
        /// </summary>
        private readonly DiscordService _discord;
        /// <summary>
        /// Google service
        /// </summary>
        private readonly GoogleService _google;
        /// <summary>
        /// Spotify service
        /// </summary>
        private readonly SpotifyService _spotify;
        /// <summary>
        /// Twitch service
        /// </summary>
        private readonly TwitchService _twitch;

        /// <summary>
        /// Websocket context
        /// </summary>
        private readonly IHubContext<AreaHub> _hubContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mongo">Database access</param>
        /// <param name="discord">Discord service</param>
        /// <param name="google">Google service</param>
        /// <param name="spotify">Spotify service</param>
        /// <param name="twitch">Twitch service</param>
        /// <param name="hubContext">Websocket context</param>
        public AreaService(MongoService mongo, DiscordService discord, GoogleService google, SpotifyService spotify, TwitchService twitch, IHubContext<AreaHub> hubContext)
        {
            _mongo = mongo;
            _discord = discord;
            _google = google;
            _spotify = spotify;
            _twitch = twitch;

            _hubContext = hubContext;

            Schedule(async () => await Execute()).ToRunEvery(1).Minutes();
        }
       
        /// <summary>
        /// Refresh Services acess token
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns></returns>
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

        /// <summary>
        /// Execute one area from user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="id">AREA ID</param>
        /// <returns>found and executed</returns>
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

        /// <summary>
        /// Send notification to websocket that an area is updated
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="area">AREA</param>
        /// <returns></returns>
        private Task AreaExecuted(User user, Entities.Area area)
        {
            _hubContext.Clients.User(user.Id).SendAsync(area.Id, (long)(area.LastUpdate - DateTime.UnixEpoch).TotalSeconds);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Execute all area from an user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns></returns>
        public async Task ExecuteUser(User user)
        {
            await RefreshTokenBeforeExecute(user);

            foreach (Entities.Area area in user.Areas)
            {
                await area.Launch(user, _mongo);
                await AreaExecuted(user, area);
            }
            _mongo.SaveUser(user);
        }

        /// <summary>
        /// Execute all area from all users
        /// </summary>
        /// <returns></returns>
        private async Task Execute()
        {
            var users = _mongo.GetUsers();

            foreach (var user in users.Where(x => x.Areas.Count > 0))
                await ExecuteUser(user);
        }
	}
}
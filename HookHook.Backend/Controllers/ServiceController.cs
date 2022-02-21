using CoreTweet;
using Discord.Rest;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SpotifyAPI.Web;
using TwitchLib.Api;

namespace HookHook.Backend.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class ServiceController : ControllerBase
	{
		private readonly MongoService _mongo;
        private readonly DiscordService _discord;
        private readonly TwitterService _twitter;
        private readonly TwitchService _twitch;
        private readonly SpotifyService _spotify;
        private readonly IConfiguration _config;

        private readonly string _twitchId;

		public ServiceController(MongoService mongo, DiscordService discord, TwitterService twitter, TwitchService twitch, SpotifyService spotify, IConfiguration config)
		{
			_mongo = mongo;
            _discord = discord;
            _twitter = twitter;
            _twitch = twitch;
            _config = config;
            _spotify = spotify;

            _twitchId = config["Twitch:ClientId"];
		}

        [HttpGet("{provider}")]
		public async Task<ActionResult<List<ServiceAccount>>> Get(string provider)
        {
			var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
				List<ServiceAccount> list = new();
				foreach (var account in user.DiscordServices)
                {
                    await _discord.Refresh(account);

                    var client = new DiscordRestClient();
                    await client.LoginAsync(Discord.TokenType.Bearer, account.AccessToken);

                    list.Add(new(account.UserId, client.CurrentUser.ToString()));
                }
                _mongo.SaveUser(user);
				return list;
            }
            if (string.Equals(provider, "Twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                List<ServiceAccount> list = new();
                foreach (var account in user.TwitterServices)
                {
                    var client = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], account.AccessToken, account.OAuthSecret, long.Parse(account.UserId));
                    var currentUser = await client.Users.ShowAsync(client.UserId);


                    list.Add(new(account.UserId, $"@{currentUser.ScreenName}"));
                }
                _mongo.SaveUser(user);
                return list;
            }
            if (string.Equals(provider, "Twitch", StringComparison.InvariantCultureIgnoreCase))
            {
                List<ServiceAccount> list = new();
                foreach (var account in user.TwitchServices)
                {
                    await _twitch.Refresh(account);

                    var api = new TwitchAPI();
                    api.Settings.ClientId = _twitchId;
                    api.Settings.AccessToken = account.AccessToken;

                    var users = await api.Helix.Users.GetUsersAsync(null, null, account.AccessToken);

                    if (users == null)
                        throw new ApiException("Failed to call API");

                    var client = users.Users[0];

                    list.Add(new(account.UserId, client.Login));
                }
                _mongo.SaveUser(user);
                return list;
            }
            if (string.Equals(provider, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                List<ServiceAccount> list = new();
                foreach (var account in user.SpotifyServices)
                {
                    await _spotify.Refresh(account);

                    var client = new SpotifyClient(account.AccessToken);
                    var profile = await client.UserProfile.Current();


                    list.Add(new(account.UserId, profile.DisplayName));
                }
                _mongo.SaveUser(user);
                return list;
            }

            return BadRequest();
        }

		[HttpPost("{provider}")]
		public async Task<ActionResult> Add(string provider, [BindRequired] [FromQuery] string code, [FromQuery] string? verifier = null)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await _discord.AddAccount(user, code);
                    return NoContent();
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
                }
            }
            if (string.Equals(provider, "Twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await _twitter.AddAccount(user, code, verifier);
                    return NoContent();
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
                }
            }
            if (string.Equals(provider, "Twitch", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await _twitch.AddAccount(user, code);
                    return NoContent();
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
                }
            }
            if (string.Equals(provider, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await _spotify.AddAccount(user, code);
                    return NoContent();
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
                }
            }
            return BadRequest();
        }

        [HttpDelete("{provider}")]
        public async Task<ActionResult> Delete(string provider, [BindRequired] [FromQuery] string id)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.DiscordServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.DiscordServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            if (string.Equals(provider, "Twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.TwitterServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.TwitterServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            if (string.Equals(provider, "Twitch", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.TwitchServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.TwitchServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            if (string.Equals(provider, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.SpotifyServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.SpotifyServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            if (string.Equals(provider, "Google", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.GoogleServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.GoogleServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            if (string.Equals(provider, "GitHub", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.GitHubServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.GitHubServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            return BadRequest();
        }
	}
}


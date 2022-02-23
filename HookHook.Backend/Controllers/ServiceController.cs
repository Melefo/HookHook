using CoreTweet;
using Discord.Rest;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Octokit;
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
        private readonly GitHubService _gitHub;
        private readonly GoogleService _google;
        private readonly IConfiguration _config;

        private readonly string _twitchId;

		public ServiceController(MongoService mongo, DiscordService discord, TwitterService twitter, TwitchService twitch, SpotifyService spotify, GitHubService gitHub, GoogleService google, IConfiguration config)
		{
			_mongo = mongo;
            _discord = discord;
            _twitter = twitter;
            _twitch = twitch;
            _config = config;
            _spotify = spotify;
            _gitHub = gitHub;
            _google = google;

            _twitchId = config["Twitch:ClientId"];
		}

        [HttpGet("{provider}")]
        public async Task<ActionResult<List<ServiceAccount>>> Get(Providers provider)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);
            List<ServiceAccount> list = new();

            if (!user.ServicesAccounts.TryGetValue(provider, out var accounts))
                return list;
            foreach (var account in accounts)
            {
                ServiceAccount acc;
                switch (provider)
                {
                    case Providers.Discord:
                        await _discord.Refresh(account);

                        var discord = new DiscordRestClient();
                        await discord.LoginAsync(Discord.TokenType.Bearer, account.AccessToken);

                        acc = new(account.UserId, discord.CurrentUser.ToString());
                        break;
                    case Providers.Twitter:
                        var twitter = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], account.AccessToken, account.Secret, long.Parse(account.UserId));
                        var currentUser = await twitter.Users.ShowAsync(twitter.UserId);

                        acc = new(account.UserId, $"@{currentUser.ScreenName}");
                        break;
                    case Providers.Twitch:
                        await _twitch.Refresh(account);

                        var api = new TwitchAPI();
                        api.Settings.ClientId = _twitchId;
                        api.Settings.AccessToken = account.AccessToken;

                        var res = await api.Helix.Users.GetUsersAsync(null, null, account.AccessToken);

                        if (res == null)
                            throw new Exceptions.ApiException("Failed to call API");

                        acc = new(account.UserId, res.Users.SingleOrDefault()!.Login);
                        break;
                    case Providers.Spotify:
                        await _spotify.Refresh(account);

                        var spotify = new SpotifyClient(account.AccessToken);
                        var profile = await spotify.UserProfile.Current();

                        acc = new(account.UserId, profile.DisplayName);
                        break;
                    case Providers.Google:
                        await _google.Refresh(account);

                        var youtube = _google.CreateYouTube(account);
                        var req = youtube.Channels.List("snippet");
                        req.Mine = true;
                        var search = req.Execute();

                        acc = new(account.UserId, search.Items[0].Snippet.Title);
                        break;
                    case Providers.GitHub:
                        var github = new GitHubClient(new ProductHeaderValue("HookHook")); 
                        github.Credentials = new Credentials(account.AccessToken);
                        var current = await github.User.Current();

                        acc = new(current.Id.ToString(), current.Login);
                        break;
                    default:
                        return BadRequest();
                }
                list.Add(acc);
            }
            _mongo.SaveUser(user);
            return list;
        }

        [HttpPost("{provider}")]
        public async Task<ActionResult<ServiceAccount>> Add(Providers provider, [BindRequired][FromQuery] string code, [FromQuery] string? verifier = null)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);
            try
            {
                ServiceAccount? account = null;
                switch (provider)
                {
                    case Providers.Discord:
                        account = await _discord.AddAccount(user, code);
                        break;
                    case Providers.Twitter:
                        account = await _twitter.AddAccount(user, code, verifier);
                        break;
                    case Providers.Twitch:
                        account = await _twitch.AddAccount(user, code);
                        break;
                    case Providers.Spotify:
                        account = await _spotify.AddAccount(user, code);
                        break;
                    case Providers.Google:
                        account = await _google.AddAccount(user, code);
                        break;
                    case Providers.GitHub:
                        account = await _gitHub.AddAccount(user, code);
                        break;
                    default:
                        return BadRequest();
                }
                _mongo.SaveUser(user);
                if (account != null)
                    return account;
                return NoContent();
            }
            catch (Exceptions.ApiException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
            }
        }

        [HttpDelete("{provider}")]
        public ActionResult Delete(Providers provider, [BindRequired] [FromQuery] string id)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);
            if (!user.ServicesAccounts.TryGetValue(provider, out var accounts))
                return NoContent();
            var account = accounts.SingleOrDefault(x => x.UserId == id);

            accounts.Remove(account);
            _mongo.SaveUser(user);

            return NoContent();
        }
    }
}


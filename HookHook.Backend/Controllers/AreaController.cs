using HookHook.Backend.Area;
using HookHook.Backend.Area.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Area.Reactions;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HookHook.Backend.Models;
using HookHook.Backend.Attributes;
using System.Reflection;
using HookHook.Backend.Utilities;
using System.Linq;

namespace HookHook.Backend.Controllers
{
    /// <summary>
    /// /area controller route
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AreaController : ControllerBase
    {
        private readonly MongoService _db;
        private readonly AreaService _area;

        public Dictionary<string, Func<string[], string, User, IAction>> actionTypes;
        public Dictionary<string, Func<string[], string, User, IReaction>> reactionTypes;

        public AreaController(MongoService db, TwitterService twitterService, GoogleService googleService, AreaService area, IConfiguration config)
        {
            _db = db;
            _area = area;

            actionTypes = new()
            {
                { nameof(DiscordPinned), (string[] args, string accountId, User user) => new DiscordPinned(ulong.Parse(args[0]), ulong.Parse(args[1]), accountId, config["Discord:BotToken"]) },
                { nameof(GitHubIssueCreated), (string[] args, string accountId, User user) => new GitHubIssueCreated(args[0], args[1], accountId, user) },
                { nameof(GitHubNewCommit), (string[] args, string accountId, User user) => new GitHubNewCommit(args[0], args[1], accountId, user) },
                { nameof(GitHubNewRepository), (string[] args, string accountId, User user) => new GitHubNewRepository(args[0], accountId, user) },
                { nameof(SpotifyLikedAlbum), (string[] args, string accountId, User user) => new SpotifyLikedAlbum(accountId, user) },
                { nameof(SpotifyLikedMusic), (string[] args, string accountId, User user) => new SpotifyLikedMusic(accountId, user) },
                { nameof(TwitchLiveStarted), (string[] args, string accountId, User user) => new TwitchLiveStarted(args[0], accountId, config["Twitch:ClientId"]) },
                { nameof(TwitchFollowChannel), (string[] args, string accountId, User user) => new TwitchFollowChannel(args[0], accountId, user, config["Twitch:ClientId"]) },
                { nameof(TwitterFollowUser), (string[] args, string accountId, User user) => new TwitterFollowUser(args[0], accountId, user, config["Twitter:ClientId"], config["Twitter:ClientSecret"]) },
                { nameof(TwitterTweetHashtag), (string[] args, string accountId, User user) => new TwitterTweetHashtag(args[0], accountId, user, config["Twitter:ClientId"], config["Twitter:ClientSecret"]) },
                { nameof(YoutubeVideoPublished), (string[] args, string accountId, User user) => new YoutubeVideoPublished(args[0], accountId, user, googleService) }
            };

            reactionTypes = new()
            {
                { nameof(DiscordWebhook), (string[] args, string accountId, User user) => new DiscordWebhook(args[0], args[1], args[2], accountId) },
                { nameof(GithubCreateRepository), (string[] args, string accountId, User user) => new GithubCreateRepository(args[0], args[1], accountId) },
                { nameof(GithubCreateIssue), (string[] args, string accountId, User user) => new GithubCreateIssue(args[0], args[1], args[2], args[3], accountId) },
                { nameof(SpotifyLikeAlbum), (string[] args, string accountId, User user) => new SpotifyLikeAlbum(args[0], args[1], accountId) },
                { nameof(SpotifyLikeMusic), (string[] args, string accountId, User user) => new SpotifyLikeMusic(args[0], args[1], accountId) },
                { nameof(TwitterFollowUser), (string[] args, string accountId, User user) => new TwitterFollowUser(args[0], accountId, user, config["Twitter:ClientId"], config["Twitter:ClientSecret"]) },
                { nameof(TwitterTweet), (string[] args, string accountId, User user) => new TwitterTweet(args[0], accountId, config["Twitter:ClientId"], config["Twitter:ClientSecret"]) },
                { nameof(YoutubePostComment), (string[] args, string accountId, User user) => new YoutubePostComment(args[0], args[1], googleService, accountId) }
            };
        }

        /// <summary>
        /// Create entity from model
        /// </summary>
        /// <param name="area"></param>
        /// <param name="user"></param>
        /// <returns>Area entity</returns>
        private Entities.Area CreateEntityFromModel(AreaModel area, User user)
        {
            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments, area.Action.AccountId, user);

            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++)
                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments, area.Reactions[i].AccountId, user));

            // * create an area entity
            Entities.Area areaEntity = new Entities.Area(area.Name, action, reactions, area.Minutes);

            return areaEntity;
        }

        private class ServiceDescription
        {
            public Providers Name { get; set; }
            public string ClassName { get; set; }
            public string Description { get; set; }

            public string[] ParameterNames { get; set; }
            public string[]? Formatters { get; set; }

            public string AreaType { get; set; } // * ACTION or REACTION

            public ServiceDescription(Providers name, string className, string description, string type, string[] parameters, string[]? formatters)
            {
                Name = name;
                ClassName = className;
                Description = description;
                AreaType = type;
                ParameterNames = parameters;
                Formatters = formatters;
            }
        }

        /// <summary>
        /// Get services
        /// </summary>
        /// <returns>Array of services (with parameters, formatters, etc...)</returns>
        [HttpGet("getServices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getServices()
        {
            // * retrieve classes that have the Service attribute, get their constructor and argument list
            var services = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

            // * response is an array containing:
            List<ServiceDescription> servicesResponse = new();

            var stringType = typeof(string);
            var reactionType = typeof(IReaction);
            var actionType = typeof(IAction);

            foreach (var service in services)
            {
                var constructor = service.GetConstructors().SingleOrDefault(x => x.GetParameters().Any(x => x.GetCustomAttribute<ParameterNameAttribute>() != null));
                var parameters = constructor?.GetParameters();
                var @params = parameters?.Where(x => x.GetCustomAttribute<ParameterNameAttribute>() != null).Select(x => x.GetCustomAttribute<ParameterNameAttribute>()!.Name).ToArray();
                @params ??= Array.Empty<string>();

                var attr = service.GetCustomAttribute<ServiceAttribute>()!;

                if (actionType.IsAssignableFrom(service))
                {
                    string[]? formatters = (string[]?)service.GetProperty("Formatters")?.GetValue(null);
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Action", @params, formatters));
                }
                if (reactionType.IsAssignableFrom(service))
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Reaction", @params, null));
            }
            return Ok(servicesResponse);
        }


        /// <summary>
        /// Create a new area
        /// </summary>
        /// <param name="area"></param>
        /// <returns>The created area</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserArea> CreateArea([FromBody] AreaModel area)
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);

            if (user == null)
                return BadRequest();

            // * if account id/type is missing, reaction will be set to null
            if (Array.Exists(area.Reactions, reaction => reaction == null)) {
                return (BadRequest(new {
                    error = "Please fill out reaction details"
                }));
            }

            // * check for empty arguments
            if (Array.Exists(area.Action.Arguments, arg => arg == null) || Array.Exists(area.Action.Arguments, arg => arg == "")) {
                return BadRequest(new {
                    error = "Please fill out all the action arguments"
                });
            }
            // * check for invalid/empty reaction
            foreach (var reaction in area.Reactions) {

                if (Array.Exists(reaction.Arguments, arg => arg == null) || Array.Exists(reaction.Arguments, arg => arg == "")) {
                    return BadRequest(new {
                        error = "Please fill out all the reaction arguments"
                    });
                }
                if (String.IsNullOrEmpty(reaction.AccountId)) {
                    return BadRequest(new {
                        error = "Please select an account for your reaction"
                    });
                }
            }

            Entities.Area areaEntity = CreateEntityFromModel(area, user);

            user.Areas.Add(areaEntity);
            _db.SaveUser(user);

            UserArea userArea = new(areaEntity.Id, areaEntity.Name, areaEntity.Action.GetProvider(), areaEntity.Reactions.Select(x => x.GetProvider()).ToArray(), areaEntity.LastUpdate, areaEntity.LastLaunchFailed);
            return Ok(userArea);
        }

        /// <summary>
        /// Modify an area
        /// </summary>
        /// <param name="area"></param>
        /// <param name="id"></param>
        /// <returns>The modified area</returns>
        [HttpPut("modify/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ModifyArea([FromBody] AreaModel area, string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
            if (user == null)
                return BadRequest();

            // * find the AREA to modify and replace it...
            var areaEntity = user.Areas.SingleOrDefault(x => x.Id == id);
            if (areaEntity == null)
                return BadRequest();

            user.Areas.Remove(areaEntity);
            user.Areas.Add(CreateEntityFromModel(area, user));
            _db.SaveUser(user);

            return Ok(areaEntity);
        }

        /// <summary>
        /// Delete an area
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteArea(string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
            if (user == null)
                return BadRequest();

            var area = user.Areas.SingleOrDefault(x => x.Id == id);
            if (area == null)
                return BadRequest();

            user.Areas.Remove(area);
            _db.SaveUser(user);

            return NoContent();
        }

        /// <summary>
        /// Trigger the areas of a user
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("trigger/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TriggerUserAreas(string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
            if (user == null)
                return BadRequest();
            if (!await _area.ExecuteUserArea(user, id))
                return BadRequest();
            return NoContent();
        }

        public class UserArea
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Providers From { get; set; }
            public Providers[] To { get; set; }
            public long Date { get; set; }

            public bool LastLaunchFailed { get; set; }

            public UserArea(string id, string name, Providers from, Providers[] to, DateTime date, bool lastLaunchFailed)
            {
                Id = id;
                Name = name;
                From = from;
                To = to;
                Date = (long)(date - DateTime.UnixEpoch).TotalSeconds;
                LastLaunchFailed = lastLaunchFailed;
            }
        }

        /// <summary>
        /// Get the areas of a user
        /// </summary>
        /// <returns>The areas</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<UserArea>> GetAllUserAreas()
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
            if (user == null)
                return BadRequest();

            List<UserArea> list = new();
            foreach (var area in user.Areas)
                list.Add(new(area.Id, area.Name, area.Action.GetProvider(), area.Reactions.Select(x => x.GetProvider()).ToArray(), area.LastUpdate, area.LastLaunchFailed));

            return list;
        }

    }
}
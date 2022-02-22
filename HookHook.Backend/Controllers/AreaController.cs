using HookHook.Backend.Area;
using HookHook.Backend.Area.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Area.Reactions;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HookHook.Backend.Actions;
using HookHook.Backend.Reactions;
using HookHook.Backend.Models;
using HookHook.Backend.Attributes;
using System.Reflection;

namespace HookHook.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AreaController : ControllerBase
    {
        public MongoService _db;

        public Dictionary<string, Func<string[], IAction>> actionTypes = new();
        public Dictionary<string, Func<string[], IReaction>> reactionTypes = new();

        public AreaController(MongoService db, TwitterService twitterService, GoogleService googleService, IConfiguration config)
        {
            _db = db;

            actionTypes.Add("DiscordPinned", (string[] args) => new DiscordPinned(args[0], args[1]));
            actionTypes.Add("GithubIssueCreated", (string[] args) => new GithubIssueCreated(args[0], args[1]));
            actionTypes.Add("GithubNewCommit", (string[] args) => new GithubNewCommit(args[0], args[1]));
            actionTypes.Add("GithubNewRepository", (string[] args) => new GithubNewRepository(args[0]));
            actionTypes.Add("SpotifyLikeAlbum", (string[] args) => new SpotifyLikeAlbum(args[0], args[1]));
            actionTypes.Add("SpotifyLikeMusic", (string[] args) => new SpotifyLikeMusic(args[0], args[1]));
            actionTypes.Add("TwitchLiveStarted", (string[] args) => new TwitchLiveStarted(args[0]));
            actionTypes.Add("TwitchFollowChannel", (string[] args) => new TwitchFollowChannel(args[0]));
            actionTypes.Add("TwitterFollowUser", (string[] args) => new TwitterFollowUser(args[0], twitterService, config));
            actionTypes.Add("TwitterTweetHashtag", (string[] args) => new TwitterTweetHashtag(args[0], config));
            actionTypes.Add("YoutubeVideoPublished", (string[] args) => new YoutubeVideoPublished(args[0], googleService));

            reactionTypes.Add("DiscordWebhook", (string[] args) => new DiscordWebhook(args[0], args[1]));
            reactionTypes.Add("GithubCreateRepository", (string[] args) => new GithubCreateRepository(args[0], args[1]));
            reactionTypes.Add("GithubCreateIssue", (string[] args) => new GithubCreateIssue(args[0], args[1], args[2], args[3]));
            reactionTypes.Add("SpotifyLikeAlbum", (string[] args) => new SpotifyLikeAlbum(args[0], args[1]));
            reactionTypes.Add("SpotifyLikeMusic", (string[] args) => new SpotifyLikeMusic(args[0], args[1]));
            reactionTypes.Add("TwitchFollowChannel", (string[] args) => new TwitchFollowChannel(args[0]));
            reactionTypes.Add("TwitterFollowUser", (string[] args) => new TwitterFollowUser(args[0], twitterService, config));
            reactionTypes.Add("TwitterTweetHashtag", (string[] args) => new TwitterTweetHashtag(args[0], config, args[1]));
            reactionTypes.Add("YoutubePostComment", (string[] args) => new YoutubePostComment(args[0], args[1], googleService));

        }

        private Entities.Area CreateEntityFromModel(AreaModel area)
        {
            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments);

            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++) {
                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments));
            }

            // * create an area entity
            Entities.Area areaEntity = new Entities.Area(action, reactions, area.Minutes);
            return (areaEntity);
        }

        private class ServiceDescription
        {
            public string Name { get; set; }
            public string ClassName { get; set; }
            public string Description { get; set; }
            // public int parameterCount { get; set; }

            public string[] ParameterNames { get; set; }

            public string AreaType { get; set; } // * ACTION or REACTION

            public ServiceDescription(string name, string className, string description, string type, params string[] parameters)
            {
                Name = name;
                ClassName = className;
                Description = description;
                AreaType = type;
                ParameterNames = parameters;
            }
        }

        [HttpGet("getServices")]
        public async Task<ActionResult> getServices()
        {
            // * retrieve classes that have the Service attribute, get their constructor and argument list
            var services = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

            // * response is an array containing:
            List<ServiceDescription> servicesResponse = new();

            var stringType = typeof(string);
            var reactionType = typeof(IReaction);
            var actionType = typeof(IAction);

            foreach (var service in services) {

                var parameters = service.GetConstructors()[0].GetParameters();
                var strParams = parameters.Where(x => x.ParameterType == stringType).ToArray();

                var attr = service.GetCustomAttribute<ServiceAttribute>();

                if (actionType.IsAssignableFrom(service))
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Action", strParams.Select(x => x.Name).ToArray()));
                if (reactionType.IsAssignableFrom(service))
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Reaction", strParams.Select(x => x.Name).ToArray()));
            }
            return Ok(servicesResponse);
        }


        // * create a new area
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CreateArea([FromBody] AreaModel area)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            Entities.Area areaEntity = CreateEntityFromModel(area);

            user.Areas.Add(areaEntity);
            _db.SaveUser(user);

            return Ok(areaEntity);
        }

        // * modify -> add/remove reactions/action, so a new area ??
        // * PUT vu qu'on envoie un nouveau AREA je dirais
        [HttpPut("modify/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ModifyArea([FromBody] AreaModel area, string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            // * find the AREA to modify and replace it...
            var areaEntity = user.Areas.SingleOrDefault(x => x.Id == id);
            if (areaEntity == null)
                return BadRequest();

            user.Areas.Remove(areaEntity);
            user.Areas.Add(CreateEntityFromModel(area));
            _db.SaveUser(user);

            return Ok(areaEntity);
        }

        // * delete -> rm area by ID
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteArea(string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            var area = user.Areas.SingleOrDefault(x => x.Id == id);
            if (area == null)
                return BadRequest();

            user.Areas.Remove(area);
            _db.SaveUser(user);

            return NoContent();
        }

        // * trigger all areas
        [HttpGet("Trigger/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TriggerUserAreas(string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            foreach (var area in user.Areas)
                await area.Launch(user);
            _db.SaveUser(user);
            
            return Ok();
        }
    }
}

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

        public Dictionary<string, Func<string[], string, Entities.User, IAction>> actionTypes = new();
        public Dictionary<string, Func<string[], string, IReaction>> reactionTypes = new();

        public AreaController(MongoService db, TwitterService twitterService, GoogleService googleService, IConfiguration config)
        {
            _db = db;

            actionTypes.Add("DiscordPinned", (string[] args, string accountId, Entities.User user) => new DiscordPinned(args[0], args[1], accountId));
            actionTypes.Add("GithubIssueCreated", (string[] args, string accountId, Entities.User user) => new GithubIssueCreated(args[0], args[1], accountId, _db, user));
            actionTypes.Add("GithubNewCommit", (string[] args, string accountId, Entities.User user) => new GithubNewCommit(args[0], args[1], accountId));
            actionTypes.Add("GithubNewRepository", (string[] args, string accountId, Entities.User user) => new GithubNewRepository(args[0], accountId));
            actionTypes.Add("SpotifyLikeAlbum", (string[] args, string accountId, Entities.User user) => new SpotifyLikeAlbum(args[0], args[1], accountId));
            actionTypes.Add("SpotifyLikeMusic", (string[] args, string accountId, Entities.User user) => new SpotifyLikeMusic(args[0], args[1], accountId));
            actionTypes.Add("TwitchLiveStarted", (string[] args, string accountId, Entities.User user) => new TwitchLiveStarted(args[0], accountId));
            actionTypes.Add("TwitchFollowChannel", (string[] args, string accountId, Entities.User user) => new TwitchFollowChannel(args[0], accountId));
            actionTypes.Add("TwitterFollowUser", (string[] args, string accountId, Entities.User user) => new TwitterFollowUser(args[0], twitterService, config, accountId));
            actionTypes.Add("TwitterTweetHashtag", (string[] args, string accountId, Entities.User user) => new TwitterTweetHashtag(args[0], config, accountId));
            actionTypes.Add("YoutubeVideoPublished", (string[] args, string accountId, Entities.User user) => new YoutubeVideoPublished(args[0], googleService, accountId));

            reactionTypes.Add("DiscordWebhook", (string[] args, string accountId) => new DiscordWebhook(args[0], args[1]));
            reactionTypes.Add("GithubCreateRepository", (string[] args, string accountId) => new GithubCreateRepository(args[0], args[1], accountId));
            reactionTypes.Add("GithubCreateIssue", (string[] args, string accountId) => new GithubCreateIssue(args[0], args[1], args[2], args[3], accountId));
            reactionTypes.Add("SpotifyLikeAlbum", (string[] args, string accountId) => new SpotifyLikeAlbum(args[0], args[1], accountId));
            reactionTypes.Add("SpotifyLikeMusic", (string[] args, string accountId) => new SpotifyLikeMusic(args[0], args[1], accountId));
            reactionTypes.Add("TwitchFollowChannel", (string[] args, string accountId) => new TwitchFollowChannel(args[0], accountId));
            reactionTypes.Add("TwitterFollowUser", (string[] args, string accountId) => new TwitterFollowUser(args[0], twitterService, config, accountId));
            reactionTypes.Add("TwitterTweetHashtag", (string[] args, string accountId) => new TwitterTweetHashtag(args[0], config, args[1], accountId));
            reactionTypes.Add("YoutubePostComment", (string[] args, string accountId) => new YoutubePostComment(args[0], args[1], googleService, accountId));

        }

        private Entities.Area CreateEntityFromModel(AreaModel area, Entities.User user)
        {
            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments, area.Action.AccountID, user);

            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++) {
                Console.WriteLine(area.Reactions[i].AccountID);

                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments, area.Reactions[i].AccountID));
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

            public string[] parameterNames { get; set; }

            public string areaType { get; set; } // * ACTION or REACTION
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

                // * get controller
                // ! code bancal
                var parameters = service.GetConstructors()[0].GetParameters();
                // * on esquive le serviceAccountId, c'est pas au user de le rentrer
                var strParams = parameters.Where(x => x.ParameterType == stringType && x.Name != "serviceAccountId").ToArray();

                var attr = service.GetCustomAttribute<ServiceAttribute>();
                var newService = new ServiceDescription();
                newService.Name = attr.Name;
                newService.Description = attr.Description;
                // newService.parameterCount = strParams.Length;
                newService.parameterNames = strParams.Select(x => x.Name).ToArray();
                newService.ClassName = service.Name;

                if (actionType.IsAssignableFrom(service) && reactionType.IsAssignableFrom(service))
                    newService.areaType = "Action/Reaction";
                else if (actionType.IsAssignableFrom(service))
                    newService.areaType = "Action";
                else if (reactionType.IsAssignableFrom(service))
                    newService.areaType = "Reaction";

                servicesResponse.Add(newService);
            }
            return (Ok(servicesResponse));
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

            Entities.Area areaEntity = CreateEntityFromModel(area, user);

            Console.WriteLine("About to add areaEntity");
            user.Areas.Add(areaEntity);
            Console.WriteLine("About to save");
            _db.SaveUser(user);

            Console.WriteLine("Done");
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
            user.Areas.Add(CreateEntityFromModel(area, user));
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
            var user = _db.GetUser(id);
            if (user == null)
                return BadRequest();

            foreach (var area in user.Areas)
                await area.Launch(user);
            _db.SaveUser(user);
            return Ok();
        }

        // * get all the areas
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllUserAreas()
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            List<Entities.Area> areas = new();

            foreach (var area in user.Areas) {
                areas.Append(area);
            }

            return Ok(areas);
        }

    }
}

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
using HookHook.Backend.Exceptions;
using System.Reflection;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AreaController : ControllerBase
    {
        public MongoService _db;

        public Dictionary<string, Func<string[], string, User, IAction>> actionTypes = new();
        public Dictionary<string, Func<string[], string, IReaction>> reactionTypes = new();

        public AreaController(MongoService db, TwitterService twitterService, GoogleService googleService, IConfiguration config)
        {
            _db = db;

            actionTypes.Add("DiscordPinned", (string[] args, string accountId, User user) => new DiscordPinned(args[0], args[1], accountId));
            actionTypes.Add("GithubIssueCreated", (string[] args, string accountId, User user) => new GithubIssueCreated(args[0], args[1], accountId, _db, user));
            actionTypes.Add("GithubNewCommit", (string[] args, string accountId, User user) => new GithubNewCommit(args[0], args[1], accountId));
            actionTypes.Add("GithubNewRepository", (string[] args, string accountId, User user) => new GithubNewRepository(args[0], accountId));
            actionTypes.Add("SpotifyLikeAlbum", (string[] args, string accountId, User user) => new SpotifyLikeAlbum(args[0], args[1], accountId));
            actionTypes.Add("SpotifyLikeMusic", (string[] args, string accountId, User user) => new SpotifyLikeMusic(args[0], args[1], accountId));
            actionTypes.Add("TwitchLiveStarted", (string[] args, string accountId, User user) => new TwitchLiveStarted(args[0], accountId));
            actionTypes.Add("TwitchFollowChannel", (string[] args, string accountId, User user) => new TwitchFollowChannel(args[0], accountId));
            actionTypes.Add("TwitterFollowUser", (string[] args, string accountId, User user) => new TwitterFollowUser(args[0], twitterService, config, accountId));
            actionTypes.Add("TwitterTweetHashtag", (string[] args, string accountId, User user) => new TwitterTweetHashtag(args[0], config, accountId));
            actionTypes.Add("YoutubeVideoPublished", (string[] args, string accountId, User user) => new YoutubeVideoPublished(args[0], googleService, accountId));

            reactionTypes.Add("DiscordWebhook", (string[] args, string accountId) => new DiscordWebhook(args[0], args[1]));
            reactionTypes.Add("GithubCreateRepository", (string[] args, string accountId) => new GithubCreateRepository(args[0], args[1], accountId));
            reactionTypes.Add("GithubCreateIssue", (string[] args, string accountId) => new GithubCreateIssue(args[0], args[1], args[2], args[3], accountId));
            reactionTypes.Add("SpotifyLikeAlbum", (string[] args, string accountId) => new SpotifyLikeAlbum(args[0], args[1], accountId));
            reactionTypes.Add("SpotifyLikeMusic", (string[] args, string accountId) => new SpotifyLikeMusic(args[0], args[1], accountId));
            reactionTypes.Add("TwitchFollowChannel", (string[] args, string accountId) => new TwitchFollowChannel(args[0], accountId));
            reactionTypes.Add("TwitterFollowUser", (string[] args, string accountId) => new TwitterFollowUser(args[0], twitterService, config, accountId));
            reactionTypes.Add("TwitterTweetHashtag", (string[] args, string accountId) => new TwitterTweetHashtag(args[0], config, accountId, args[1]));
            reactionTypes.Add("YoutubePostComment", (string[] args, string accountId) => new YoutubePostComment(args[0], args[1], googleService, accountId));

        }

        private Entities.Area CreateEntityFromModel(AreaModel area, Entities.User user)
        {
            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments, area.Action.AccountId, user);

            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++) {
                Console.WriteLine(area.Reactions[i].AccountId);

                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments, area.Reactions[i].AccountId));
            }

            // * create an area entity
            Entities.Area areaEntity = new Entities.Area(area.Name, action, reactions, area.Minutes);

            return (areaEntity);
        }

        private class ServiceDescription
        {
            public Providers Name { get; set; }
            public string ClassName { get; set; }
            public string Description { get; set; }
            // public int parameterCount { get; set; }

            public string[] ParameterNames { get; set; }

            public string AreaType { get; set; } // * ACTION or REACTION

            public ServiceDescription(Providers name, string className, string description, string type, params string[] parameters)
            {
                Name = name;
                ClassName = className;
                Description = description;
                AreaType = type;
                ParameterNames = parameters;
            }
        }

        [HttpGet("getServices")]
        public ActionResult getServices()
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
                // * on esquive le serviceAccountId, c'est pas au user de le rentrer
                var strParams = parameters.Where(x => x.ParameterType == stringType && x.Name != "serviceAccountId").ToArray();

                var attr = service.GetCustomAttribute<ServiceAttribute>()!;

                if (actionType.IsAssignableFrom(service))
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Action", strParams.Select(x => x.Name).ToArray()!));
                if (reactionType.IsAssignableFrom(service))
                    servicesResponse.Add(new(attr.Name, service.Name, attr.Description, "Reaction", strParams.Select(x => x.Name).ToArray()!));
            }
            return Ok(servicesResponse);
        }


        // * create a new area
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CreateArea([FromBody] AreaModel area)
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
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

        // * delete -> rm area by ID
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
                await area.Launch(user, _db);
            _db.SaveUser(user);
            return Ok();
        }

        public class Test
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Providers From { get; set; }
            public Providers[] To { get; set; }
            public long Date { get; set; }

            public Test(string id, string name, Providers from, Providers[] to, DateTime date)
            {
                Id = id;
                Name = name;
                From = from;
                To = to;
                Date = (long)(date - DateTime.UnixEpoch).TotalSeconds;
            }
        }

        // * get all the areas
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Test>> GetAllUserAreas()
        {
            var user = _db.GetUser(HttpContext.User.Identity!.Name!);
            if (user == null)
                return BadRequest();

            List<Test> list = new();
            foreach (var area in user.Areas)
                list.Add(new(area.Id, area.Name, area.Action.GetProvider(), area.Reactions.Select(x => x.GetProvider()).ToArray(), area.LastUpdate));

            return list;
        }

    }
}

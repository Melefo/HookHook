using HookHook.Backend.Area;
using HookHook.Backend.Area.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Area.Reactions;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HookHook.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        public MongoService _db;

        public Dictionary<string, Func<string[], IAction>> actionTypes = new();
        public Dictionary<string, Func<string[], IReaction>> reactionTypes = new();

        public AreaController(MongoService db, IConfiguration config)
        {
            _db = db;

            actionTypes.Add("DiscordPinned", (string[] args) => new DiscordPinned(ulong.Parse(args[0]), ulong.Parse(args[1])));
            actionTypes.Add("GithubIssueCreated", (string[] args) => new GithubIssueCreated(args[0], args[1]));
            actionTypes.Add("GithubNewCommit", (string[] args) => new GithubNewCommit(args[0], args[1]));
            actionTypes.Add("GithubNewRepository", (string[] args) => new GithubNewRepository(args[0]));
            actionTypes.Add("SpotifyLikeAlbum", (string[] args) => new SpotifyLikeAlbum(args[0], args[1]));
            actionTypes.Add("SpotifyLikeMusic", (string[] args) => new SpotifyLikeMusic(args[0], args[1]));
            actionTypes.Add("TwitchLiveStarted", (string[] args) => new TwitchLiveStarted(args[0]));
            actionTypes.Add("TwitchFollowChannel", (string[] args) => new TwitchFollowChannel(args[0]));

            reactionTypes.Add("DiscordWebhook", (string[] args) => new DiscordWebhook(args[0], args[1]));
            reactionTypes.Add("GithubCreateRepository", (string[] args) => new GithubCreateRepository(args[0], args[1]));
            reactionTypes.Add("GithubCreateIssue", (string[] args) => new GithubCreateIssue(args[0], args[1], args[1], args[2]));
            reactionTypes.Add("SpotifyLikeAlbum", (string[] args) => new SpotifyLikeAlbum(args[0], args[1]));
            reactionTypes.Add("SpotifyLikeMusic", (string[] args) => new SpotifyLikeMusic(args[0], args[1]));
            reactionTypes.Add("TwitchFollowChannel", (string[] args) => new TwitchFollowChannel(args[0]));
        }

        Entities.Area CreateEntityFromModel(Models.Area area)
        {
            Console.WriteLine("OKAY HERE");
            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments);
            Console.WriteLine("OKAY HERE");

            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++) {
                Console.WriteLine($"Reaction {i} has {area.Reactions[i].Arguments.Length} args");

                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments));
            }
            Console.WriteLine("OKAY HERE");

            // * create an area entity
            Entities.Area areaEntity = new Entities.Area(action, reactions, area.Minutes);
            return (areaEntity);
        }

        // * create a new area
        [HttpPost("create")]
        public async Task<ActionResult> CreateArea([FromBody] Models.Area area)
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
        public async Task<ActionResult> ModifyArea([FromBody] Models.Area area, string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return (BadRequest());

            // * find the AREA to modify and replace it...
            for (int i = 0; i < user.Areas.Count; i++) {
                if (user.Areas[i].Id == id) {
                    Entities.Area areaEntity = CreateEntityFromModel(area);

                    user.Areas[i] = areaEntity;
                    _db.SaveUser(user);

                    return (Ok(areaEntity));
                }
            }
            return (BadRequest());
        }

        // * delete -> rm area by ID
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteArea(string id)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return (BadRequest());

            // * il doit y avoir moyen plus stylé mais que vous voulez vous
            for (int i = 0; i < user.Areas.Count; i++) {
                if (user.Areas[i].Id == id) {
                    user.Areas.RemoveAt(i);
                    return(Ok());
                }
            }
            return (BadRequest());
        }

        // * trigger all areas
        [HttpGet("TriggerAll")]
        public async Task<ActionResult> TriggerAreas()
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            Console.WriteLine("AFTER USER GET");

            for (int i = 0; i < user.Areas.Count; i++) {
                // * bon pour le temps entre chaque lancement je sais pas si tu veux check ici ou dans Launch
                await user.Areas[i].Launch(user);
            }
            return Ok();
        }


        [Authorize]
        [HttpPost("poc")]
        public async Task<ActionResult> POC(string githubUsername, string repository, string discordWebhook)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();
            var action = new GithubIssueCreated(githubUsername, repository);
            (string? commit, bool res) = await action.Check(user);

            if (!res)
                BadRequest();
            var reaction = new DiscordWebhook(discordWebhook, commit);
            await reaction.Execute(user);
            return Ok();
        }
    }
}

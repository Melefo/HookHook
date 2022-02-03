using HookHook.Backend.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Reactions;
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

        public AreaController(MongoService db)
        {
            _db = db;

            actionTypes.Add("DiscordPinned", (string[] args) => new DiscordPinned(ulong.Parse(args[0]), ulong.Parse(args[1])));
            actionTypes.Add("GithubIssueCreated", (string[] args) => new GithubIssueCreated(args[0], args[1]));
            actionTypes.Add("GithubNewCommit", (string[] args) => new GithubNewCommit(args[0], args[1]));
            actionTypes.Add("GithubNewRepository", (string[] args) => new GithubNewRepository(args[0]));

            reactionTypes.Add("DiscordWebhook", (string[] args) => new DiscordWebhook(args[0], args[1]));
            reactionTypes.Add("GithubCreateRepository", (string[] args) => new GithubCreateRepository(args[0], args[1]));
            reactionTypes.Add("GithubCreateIssue", (string[] args) => new GithubCreateIssue(args[0], args[1], args[1], args[2]));
        }

        // * create a new area
        [HttpPost("create")]
        public async Task<ActionResult> CreateArea([FromBody] Models.Area area)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            // * create an IAction from area.Action.type
            IAction action = actionTypes[area.Action.Type](area.Action.Arguments);
            // * create list of IReactions from area.Reactions
            List<IReaction> reactions = new();
            for (int i = 0; i < area.Reactions.Length; i++) {
                reactions.Add(reactionTypes[area.Reactions[i].Type](area.Reactions[i].Arguments));
            }

            // * create an area entity and save it to the user
            Entities.Area areaEntity = new Entities.Area(action, reactions, area.Minutes);

            user.Areas.Add(areaEntity);
            _db.SaveUser(user);
            return Ok();
        }

        // * modify -> add/remove reactions/action, so a new area ??

        // * delete -> rm area by ID ??

        // * trigger all areas
        [HttpGet("TriggerAll")]
        public async Task<ActionResult> TriggerAreas()
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

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

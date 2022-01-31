using Microsoft.AspNetCore.Mvc;
using HookHook.Backend.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Reactions;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace HookHook.Backend.Controllers
{
    [Route("[controller]")]
	[ApiController]
    public class AreaController: ControllerBase
    {
         public MongoService _db;

        public AreaController(MongoService db) =>
            _db = db;


        [HttpPost("create")]
        public async Task<ActionResult> CreateArea(Area area)
        {
            var user = _db.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return BadRequest();

            // * create an action from area.Action.type

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
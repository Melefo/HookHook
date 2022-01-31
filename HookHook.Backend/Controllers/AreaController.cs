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

        public AreaController(MongoService db) =>
            _db = db;

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

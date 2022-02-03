using System.Net;
using System.Text;
using Discord;
using Discord.Rest;
using Discord.Webhook;
using HookHook.Backend.Actions;
using HookHook.Backend.Entities;
using HookHook.Backend.Reactions;
using Microsoft.AspNetCore.Mvc;

namespace HookHook.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscordController: ControllerBase
    {
        [HttpPost]
        public async Task Post(string url, string message)
        {
            var reaction = new DiscordWebhook(url, message);

            await reaction.Execute(new User("test@bonjour.xyz"));
        }
    }
}

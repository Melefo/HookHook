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
        public async Task Post(string token, ulong guild, ulong channel, string url, string message)
        {
            var user = new User
            {
                DiscordToken = token
            };
            IAction action = new DiscordPinned(guild, channel);
            await action.Check(user, new DiscordWebhook(url, message));
        }
    }
}

using Discord;
using Discord.Rest;
using Discord.Webhook;
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
            var webhook = new DiscordWebhookClient(url);
            await webhook.SendMessageAsync(message);
        }
    }
}

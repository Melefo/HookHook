using Discord.Webhook;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;

namespace HookHook.Backend.Reactions
{
    [Service("discord", "send message inside a webhook")]
    public class DiscordWebhook : IReaction
    {
        public string Url { get; private init; }
        public string Message { get; private init; }

        private DiscordWebhookClient _client;

        public DiscordWebhook(string url, string message)
        {
            Url = url;
            Message = message;
            _client = new DiscordWebhookClient(Url);
        }

        public async Task Execute(User user) =>
            await _client.SendMessageAsync(Message);
    }
}

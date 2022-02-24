using Discord.Webhook;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Area.Reactions
{
    [Service(Providers.Discord, "send message inside a webhook")]
    [BsonIgnoreExtraElements]
    public class DiscordWebhook : IReaction
    {
        public string Url { get; private init; }
        public string Message { get; private init; }

        [BsonIgnore]
        private DiscordWebhookClient _client;

        public DiscordWebhook(string url, string message)
        {
            Url = url;
            Message = message;
            _client = new DiscordWebhookClient(Url);
        }

        public async Task Execute(User user, string actionInfo) =>
            await _client.SendMessageAsync(Message);
    }
}

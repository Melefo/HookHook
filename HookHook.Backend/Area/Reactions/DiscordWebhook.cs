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
        public string AccountId { get; set; }

        private readonly DiscordWebhookClient _client;

        [BsonConstructor]
        public DiscordWebhook(string url)
        {
            Url = url;
            _client = new(url);
        }

        public DiscordWebhook([ParameterName("Webhook URL")] string url, [ParameterName("Message content")] string message, string accountId) : this(url)
        {
            Message = message;
            AccountId = accountId;
        }

        public async Task Execute(User user, string actionInfo) =>
            await _client.SendMessageAsync(Message);
    }
}

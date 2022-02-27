using Discord;
using Discord.Webhook;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using IReaction = HookHook.Backend.Entities.IReaction;

namespace HookHook.Backend.Area.Reactions
{
    [Service(Providers.Discord, "send message inside a webhook")]
    [BsonIgnoreExtraElements]
    public class DiscordWebhook : IReaction
    {
        public string Url { get; private init; }
        public string Message { get; private init; }
        public string Title { get; private init; }
        public string AccountId { get; set; }

        private readonly DiscordWebhookClient _client;

        [BsonConstructor]
        public DiscordWebhook(string url)
        {
            Url = url;
            _client = new(url);
        }

        public DiscordWebhook([ParameterName("Webhook URL")] string url, [ParameterName("Message title")] string title, [ParameterName("Message content")] string message, string accountId) : this(url)
        {
            Message = message;
            Title = title;
            AccountId = accountId;
        }

        public async Task Execute(User _, Dictionary<string, object?> formatters)
        {
            var title = Title.FormatParam(formatters);
            var message = Message.FormatParam(formatters);

            var embed = new EmbedBuilder()
            {
                Title = title,
                Description = message
            }.Build();
            await _client.SendMessageAsync(embeds: new[] { embed });
        }
    }
}
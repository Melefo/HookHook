using Discord;
using Discord.Webhook;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using IReaction = HookHook.Backend.Entities.IReaction;

namespace HookHook.Backend.Area.Reactions
{
    /// <summary>
    /// Discord Webhook reaction
    /// </summary>
    [Service(Providers.Discord, "send message inside a webhook")]
    [BsonIgnoreExtraElements]
    public class DiscordWebhook : IReaction
    {
        /// <summary>
        /// Webhook url
        /// </summary>
        public string Url { get; private init; }
        /// <summary>
        /// Discord Embed content
        /// </summary>
        public string Message { get; private init; }
        /// <summary>
        /// Discord Embed title
        /// </summary>
        public string Title { get; private init; }
        /// <summary>
        /// Discord service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Client used to check on Discord API
        /// </summary>
        private readonly DiscordWebhookClient _client;

        /// <summary>
        /// DiscordWebhook constructor used by Mongo
        /// </summary>
        /// <param name="url">Webhook URL</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public DiscordWebhook(string url)
        {
            Url = url;
            _client = new(url);
        }

        /// <summary>
        /// DiscordWebhook constructor
        /// </summary>
        /// <param name="url">Webhook URL</param>
        /// <param name="title">Discord Embed title</param>
        /// <param name="message">Discord embed content</param>
        /// <param name="accountId">Discord service account Id</param>
        public DiscordWebhook([ParameterName("Webhook URL")] string url, [ParameterName("Message title")] string title, [ParameterName("Message content")] string message, string accountId) : this(url)
        {
            Message = message;
            Title = title;
            AccountId = accountId;
        }

        /// <summary>
        /// Send message to webhook
        /// </summary>
        /// <param name="_">HookHook user</param>
        /// <param name="formatters">List of formatters from action</param>
        /// <returns></returns>
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
using Discord;
using Discord.Rest;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Discord pinned action
    /// </summary>
    [Service(Providers.Discord, "message is pinned")]
    [BsonIgnoreExtraElements]
    public class DiscordPinned : IAction
    {
        /// <summary>
        /// List of formatters used in reaction
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "msg.content", "msg.id", "author.id", "author.name", "author.mention", "msg.date"
        };

        /// <summary>
        /// Guild ID to where channel is
        /// </summary>
        public ulong GuildId { get; private init; }
        /// <summary>
        /// Channel ID to where check if a new pinned
        /// </summary>
        public ulong ChannelId { get; private init; }
        /// <summary>
        /// Service OAtuh associated with this action
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved pinned messages
        /// </summary>
        public List<ulong> PinnedMessages { get; private init; } = new();

        /// <summary>
        /// Bot token used to check on Discord API
        /// </summary>
        private readonly string _botToken;
        /// <summary>
        /// Client used to check Discord API
        /// </summary>
        private readonly DiscordRestClient _client;

        /// <summary>
        /// DiscordPinned constructor
        /// </summary>
        /// <param name="guildId">Discord Guild ID</param>
        /// <param name="channelId">Discord Channel ID</param>
        /// <param name="accountId">Discord service accoutn ID</param>
        /// <param name="botToken">Discord bot Token</param>
        public DiscordPinned([ParameterName("Guild ID")] ulong guildId, [ParameterName("Channel ID")] ulong channelId, string accountId, string botToken) : this(botToken)
        {
            GuildId = guildId;
            ChannelId = channelId;
            AccountId = accountId;

            var pinneds = GetPinned().GetAwaiter().GetResult();
            if (pinneds == null)
                return;
            foreach (var pinned in pinneds)
                PinnedMessages.Add(pinned.Id);
        }

        /// <summary>
        /// DiscordPinned constructor used by Mongo
        /// </summary>
        /// <param name="botToken">Discord bot Token</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public DiscordPinned(string botToken)
        {
            _botToken = botToken;
            _client = new();
        }

        /// <summary>
        /// Get list of pinned messages
        /// </summary>
        /// <returns>list of pinned messages</returns>
        public async Task<IReadOnlyCollection<RestMessage>?> GetPinned()
        {
            await _client.LoginAsync(TokenType.Bot, _botToken);

            var guild = await _client.GetGuildAsync(GuildId);
            if (guild == null)
                return null;

            var channel = await guild.GetTextChannelAsync(ChannelId);
            if (channel == null)
                return null;

            return await channel.GetPinnedMessagesAsync();
        }

        /// <summary>
        /// Check if a neww message is pinned
        /// </summary>
        /// <param name="_">HookHook User</param>
        /// <returns>List of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User _)
        {
            var pinneds = await GetPinned();
            if (pinneds == null)
                return (null, false);

            foreach (var message in pinneds)
            {
                if (PinnedMessages.Contains(message.Id))
                    continue;
                PinnedMessages.Add(message.Id);
                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], message.Content },
                    { Formatters[1], message.Id },
                    { Formatters[2], message.Author.Id },
                    { Formatters[3], message.Author },
                    { Formatters[4], message.Author.Mention },
                    { Formatters[5], message.CreatedAt.ToString("G") }
                };

                return (formatters, true);
            }
            return (null, false);
        }
    }
}
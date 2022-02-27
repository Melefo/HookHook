using Discord;
using Discord.Rest;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Discord, "message is pinned")]
    [BsonIgnoreExtraElements]
    public class DiscordPinned : IAction
    {
        public ulong GuildId { get; private init; }
        public ulong ChannelId { get; private init; }
        public string AccountId { get; set; }

        public string[] Formatters { get; } = new[]
        {
            "msg.content", "msg.id", "author.id", "author.name", "author.mention", "msg.date"
        };
        public List<ulong> PinnedMessages { get; private init; } = new();

        private readonly string _botToken;
        private readonly DiscordRestClient _client;

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

        public DiscordPinned(string botToken)
        {
            _botToken = botToken;
            _client = new();
        }

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
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
        public ulong Guild { get; private init; }
        public ulong Channel { get; private init; }
        public List<ulong> PinnedMessages { get; private init; } = new();

        public string AccountId { get; set; }

        [BsonIgnore]
        private DiscordRestClient _client = new();

        public DiscordPinned([ParameterName("Guild ID")] string guildId, [ParameterName("Channel ID")] string channelId, string accountId)
        {
            Guild = ulong.Parse(guildId);
            Channel = ulong.Parse(channelId);
            AccountId = accountId;

            _client = new();

            _client.LoginAsync(TokenType.Bot, AccountId).GetAwaiter().GetResult();

            var guild = _client.GetGuildAsync(Guild).GetAwaiter().GetResult();
            if (guild == null)
                return;
            var channel = guild.GetTextChannelAsync(Channel).GetAwaiter().GetResult();
            if (channel == null)
                return;

            var pinneds = channel.GetPinnedMessagesAsync().GetAwaiter().GetResult();
            foreach (var pinned in pinneds)
                PinnedMessages.Add(pinned.Id);
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _client ??= new();
            await _client.LoginAsync(TokenType.Bot, AccountId);

            var guild = await _client.GetGuildAsync(Guild);
            if (guild == null)
                return (null, false);
            var channel = await guild.GetTextChannelAsync(Channel);
            if (channel == null)
                return (null, false);

            var pinned = await channel.GetPinnedMessagesAsync();
            foreach (var message in pinned)
            {
                if (PinnedMessages.Contains(message.Id))
                    continue;

                // await reaction.Execute();
                PinnedMessages.Add(message.Id);

                // * send message.id to database ?
                return (message.Content, true);
            }
            return (null, false);
        }
    }
}

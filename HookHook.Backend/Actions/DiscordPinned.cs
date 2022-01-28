using Discord;
using Discord.Rest;
using Discord.WebSocket;
using HookHook.Backend.Entities;
using IReaction = HookHook.Backend.Entities.IReaction;

namespace HookHook.Backend.Actions
{
    public class DiscordPinned : IAction
    {
        public ulong Guild { get; private init; }
        public ulong Channel { get; private init; }
        public List<ulong> PinnedMessages { get; private init; } = new();

        private DiscordRestClient _client = new();

        public DiscordPinned(ulong guild, ulong channel)
        {
            Guild = guild;
            Channel = channel;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            await _client.LoginAsync(TokenType.Bot, user.Discord.AccessToken);

            var guild = await _client.GetGuildAsync(Guild);
            var channel = await guild.GetTextChannelAsync(Channel);

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

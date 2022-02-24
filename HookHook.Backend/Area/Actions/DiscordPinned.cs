﻿using Discord;
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

        public DiscordPinned(string guild, string channel, string serviceAccountId)
        {
            Guild = ulong.Parse(guild);
            Channel = ulong.Parse(channel);
            AccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            await _client.LoginAsync(TokenType.Bot, user.ServicesAccounts[Providers.Discord].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

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

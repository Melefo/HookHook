using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using TwitchLib.Api;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Twitch, "Check if a live started on twitch")]
    [BsonIgnoreExtraElements]
    public class TwitchLiveStarted : IAction
    {
        public static string[] Formatters { get; } = new[]
        {
            "stream.game", "stream.id", "stream.date", "stream.thumbnail", "stream.title"
        };

        public string Username { get; private init; }
        public string ClientId { get; private init; }
        public string AccountId { get; set; }

        public bool IsLive { get; private set; }

        private readonly TwitchAPI _twitchClient;

        public TwitchLiveStarted(string username, string accountId, string clientId) : this()
        {
            Username = username;
            AccountId = accountId;
            ClientId = clientId;
        }

        [BsonConstructor]
        public TwitchLiveStarted() =>
            _twitchClient = new TwitchAPI();

        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            _twitchClient.Settings.AccessToken = user.ServicesAccounts[Providers.Twitch].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken;
            _twitchClient.Settings.ClientId = ClientId;

            var streams = await _twitchClient.Helix.Streams.GetStreamsAsync(userLogins: new List<string>(){ Username });

            if (streams.Streams.Length > 0) {
                if (IsLive)
                    return (null, false);

                IsLive = true;

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], streams.Streams[0].GameName },
                    { Formatters[1], streams.Streams[0].Id },
                    { Formatters[2], streams.Streams[0].StartedAt.ToString("G") },
                    { Formatters[3], streams.Streams[0].ThumbnailUrl },
                    { Formatters[4], streams.Streams[0].Title },
                };
                return (formatters, true);
            }
            else
                IsLive = false;
            return (null, false);
        }
    }
}
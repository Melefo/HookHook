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

        public async Task<(string?, bool)> Check(User user)
        {
            _twitchClient.Settings.AccessToken = user.ServicesAccounts[Providers.Twitch].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken;
            _twitchClient.Settings.ClientId = ClientId;

            var streams = await _twitchClient.Helix.Streams.GetStreamsAsync(userLogins: new List<string>(){ Username });

            if (streams.Streams.Length > 0) {
                if (IsLive)
                    return (null, false);
                IsLive = true;
                return (streams.Streams[0].ThumbnailUrl, true);
            }
            else
                IsLive = false;
            return (null, false);
        }
    }
}
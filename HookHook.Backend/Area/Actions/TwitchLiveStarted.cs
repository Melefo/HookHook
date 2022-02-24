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
        public string UserName {get; private init;}

        [BsonIgnore]
        public TwitchAPI _twitchClient = new TwitchAPI();

        public bool isLive { get; private set; }

        public string AccountId { get; set; }
        public string _clientId { get; private init; }


        public TwitchLiveStarted(string user, string accountId, User userEntity, IConfiguration config)
        {
            UserName = user;
            isLive = false;
            AccountId = accountId;

            _clientId = config["Twitch:ClientId"];
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _twitchClient = new TwitchAPI();
            _twitchClient.Settings.AccessToken = user.ServicesAccounts[Providers.Twitch].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken;
            _twitchClient.Settings.ClientId = _clientId;

            var streams = await _twitchClient.Helix.Streams.GetStreamsAsync(userIds: new List<string>(){ UserName });

            // * on veut pas relancer si on savait deja qu'il était en stream
            if (streams.Streams.Length > 0 && isLive == false) {
                isLive = true;
                return (streams.Streams[0].ThumbnailUrl, true);
            }
            isLive = false;
            return (null, false);
        }

    }
}
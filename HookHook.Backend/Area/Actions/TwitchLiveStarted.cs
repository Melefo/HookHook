using HookHook.Backend.Utilities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using TwitchLib.Api;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;
using HookHook.Backend.Services;

namespace HookHook.Backend.Area.Actions
{
    [Service("twitch", "Check if a live started on twitch")]
    [BsonIgnoreExtraElements]
    public class TwitchLiveStarted : IAction
    {
        public string UserName {get; private init;}

        [BsonIgnore]
        public TwitchAPI _twitchClient = new TwitchAPI();

        public bool isLive { get; private set; }

        private string _serviceAccountId;

        public TwitchLiveStarted(string user, string serviceAccountId)
        {
            UserName = user;
            isLive = false;
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(Entities.User user)
        {
            _twitchClient.Settings.AccessToken = user.ServicesAccounts[Providers.Twitch].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken;

            var streams = await _twitchClient.Helix.Streams.GetStreamsAsync(userIds: new List<string>(){ UserName });

            // * on veut pas relancer si on savait deja qu'il était en stream
            if (streams.Streams.Length > 0 && isLive == false) {
                isLive = true;
                return (streams.Streams[0].ThumbnailUrl, true);
            }
            return (null, false);
        }

    }
}
using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using TwitchLib.Api;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Attributes;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Twitch live started action
    /// </summary>
    [Service(Providers.Twitch, "Check if a live started on twitch")]
    [BsonIgnoreExtraElements]
    public class TwitchLiveStarted : IAction
    {
        /// <summary>
        /// list of formatters for reaction
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "stream.game", "stream.id", "stream.date", "stream.thumbnail", "stream.title"
        };

        /// <summary>
        /// Twitch username
        /// </summary>
        public string Username { get; private init; }
        /// <summary>
        /// Twitch client Id
        /// </summary>
        public string ClientId { get; private init; }
        /// <summary>
        /// Twitch service account id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Is user currently live
        /// </summary>
        public bool IsLive { get; private set; }

        /// <summary>
        /// Client used to check on Twitch API
        /// </summary>
        private readonly TwitchAPI _twitchClient;

        /// <summary>
        /// TwitchLiveStarted constructor
        /// </summary>
        /// <param name="username">Twitch user</param>
        /// <param name="accountId">Twitch service account Id</param>
        /// <param name="clientId">Twitch client Id</param>
        public TwitchLiveStarted([ParameterName("Username")]string username, string accountId, string clientId) : this()
        {
            Username = username;
            AccountId = accountId;
            ClientId = clientId;
        }

        /// <summary>
        /// TwitchLiveStarted constructor used by Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public TwitchLiveStarted() =>
            _twitchClient = new TwitchAPI();

        /// <summary>
        /// Check if the user is currently living
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>List of formatters</returns>
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
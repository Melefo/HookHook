using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Twitch user follow channel action
    /// </summary>
    [Service(Providers.Twitch, "Follow a twitch channel")]
    [BsonIgnoreExtraElements]
    public class TwitchFollowChannel: IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "channel.name", "channel.id"
        };

        /// <summary>
        /// Twitch username
        /// </summary>
        public string Username {get; set;}
        /// <summary>
        /// Twitch Client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Twitch service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved followed users
        /// </summary>
        public List<string> FollowedUsers { get; private init; } = new();

        /// <summary>
        /// Client used to check on Twitch API
        /// </summary>
        private readonly TwitchAPI _twitchClient;

        /// <summary>
        /// TwitchFollowChannel constructor
        /// </summary>
        /// <param name="username">Twitch username</param>
        /// <param name="accountId">Twitch service account id</param>
        /// <param name="user">HookHook user</param>
        /// <param name="clientId">Twitch client Id</param>
        public TwitchFollowChannel([ParameterName("Username")] string username, string accountId, User user, string clientId) : this()
        {
            Username = username;
            AccountId = accountId;
            ClientId = clientId;

            var follows = GetUserFollows(user).GetAwaiter().GetResult();

            foreach (var follower in follows.Follows)
                FollowedUsers.Add(follower.ToUserId);
        }

        /// <summary>
        /// TwitchFollowChannel constructor used by Mongo
        /// </summary>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        [BsonConstructor]
        public TwitchFollowChannel() =>
            _twitchClient = new();

        /// <summary>
        /// Get list of all user followed
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of user</returns>
        public async Task<GetUsersFollowsResponse> GetUserFollows(User user)
        {
            var oauth = user.ServicesAccounts[Providers.Twitch].SingleOrDefault(acc => acc.UserId == AccountId)!;
            _twitchClient.Settings.AccessToken = oauth.AccessToken;
            _twitchClient.Settings.ClientId = ClientId;

            var userToCheck = await _twitchClient.Helix.Users.GetUsersAsync(logins: new List<string>()
            {
                Username
            }, accessToken: oauth.AccessToken);

            var follows = await _twitchClient.Helix.Users.GetUsersFollowsAsync(fromId: userToCheck.Users[0].Id);

            return follows;
        }

        /// <summary>
        /// Check if a new user is followed
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var follows = await GetUserFollows(user);

            foreach (var follower in follows.Follows) {
                if (FollowedUsers.Contains(follower.ToUserId))
                    continue;
                FollowedUsers.Add(follower.ToUserId);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], follower.ToUserName },
                    { Formatters[1], follower.ToUserId }
                };
                return (formatters, true);
            }
            return (null, false);
        }
    }
}
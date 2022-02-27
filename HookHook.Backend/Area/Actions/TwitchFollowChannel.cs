using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Twitch, "Follow a twitch channel")]
    [BsonIgnoreExtraElements]
    public class TwitchFollowChannel: IAction
    {
        public string Username {get; set;}
        public string ClientId { get; set; }
        public string AccountId { get; set; }

        public string[] Formatters { get; } = new[]
        {
            "channel.name", "channel.id"
        };
        public List<string> FollowedUsers { get; private init; } = new();

        private readonly TwitchAPI _twitchClient;

        public TwitchFollowChannel([ParameterName("Username")] string username, string accountId, User user, string clientId) : this()
        {
            Username = username;
            AccountId = accountId;
            ClientId = clientId;

            var follows = GetUserFollows(user).GetAwaiter().GetResult();

            foreach (var follower in follows.Follows)
                FollowedUsers.Add(follower.ToUserId);
        }

        [BsonConstructor]
        public TwitchFollowChannel() =>
            _twitchClient = new();

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
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using TwitchLib.Api;

namespace HookHook.Backend.Area
{
    [Service("twitch", "Follow a twitch channel")]
    [BsonIgnoreExtraElements]
    public class TwitchFollowChannel: IAction, IReaction
    {
        public string UserName {get; set;}

        [BsonIgnore]
        public TwitchAPI _twitchClient = new TwitchAPI();

        public List<string> FollowedUsers { get; private init; } = new();

        public TwitchFollowChannel(string user)
        {
            UserName = user;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var oauth = user.OAuthAccounts[Providers.Twitch];
            // * can we use the api with just access token ?
            _twitchClient.Settings.AccessToken = oauth.AccessToken;

            var userToCheck = await _twitchClient.Helix.Users.GetUsersAsync(logins: new List<string>() { UserName }, accessToken: oauth.AccessToken);

            var follows = await _twitchClient.Helix.Users.GetUsersFollowsAsync(fromId: userToCheck.Users[0].Id);

            foreach (var follower in follows.Follows) {

                // ! jsp si c'est toUserId ou fromUserId (followers ou comptes suivis)
                if (FollowedUsers.Contains(follower.ToUserId)) {
                    continue;
                }

                // todo save

                FollowedUsers.Add(follower.ToUserId);
                return (follower.ToUserName, true);
            }
            return (null, false);
        }

        public async Task Execute(User user)
        {
            var oauth = user.OAuthAccounts[Providers.Twitch];

            _twitchClient.Settings.AccessToken = oauth.AccessToken;

            // * search for user
            // * follow user
            var userToFollow = await _twitchClient.Helix.Users.GetUsersAsync(logins: new List<string>() { UserName}, accessToken: oauth.AccessToken);
            var authenticatedUser = await _twitchClient.Helix.Users.GetUsersAsync(accessToken: oauth.AccessToken);

            // todo gestion d'erreur

            // * j'imagine que to -> from c'est to qui suit from
            await _twitchClient.Helix.Users.CreateUserFollows(from_id: authenticatedUser.Users[0].Id, authToken: oauth.AccessToken, to_id: userToFollow.Users[0].Id);
        }
    }
}
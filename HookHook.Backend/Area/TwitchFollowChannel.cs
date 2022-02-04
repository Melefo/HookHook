using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using TwitchLib.Api;

namespace HookHook.Backend.Area
{
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
            // * can we use the api with just access token ?
            _twitchClient.Settings.AccessToken = user.TwitchOAuth.AccessToken;

            var userToCheck = await _twitchClient.Helix.Users.GetUsersAsync(logins: new List<string>() { UserName}, accessToken: user.TwitchOAuth.AccessToken);

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
            _twitchClient.Settings.AccessToken = user.TwitchOAuth.AccessToken;

            // * search for user
            // * follow user
            var userToFollow = await _twitchClient.Helix.Users.GetUsersAsync(logins: new List<string>() { UserName}, accessToken: user.TwitchOAuth.AccessToken);
            var authenticatedUser = await _twitchClient.Helix.Users.GetUsersAsync(accessToken: user.TwitchOAuth.AccessToken);

            // todo gestion d'erreur

            // * j'imagine que to -> from c'est to qui suit from
            await _twitchClient.Helix.Users.CreateUserFollows(from_id: authenticatedUser.Users[0].Id, authToken: user.TwitchOAuth.AccessToken, to_id: userToFollow.Users[0].Id)
        }
    }
}
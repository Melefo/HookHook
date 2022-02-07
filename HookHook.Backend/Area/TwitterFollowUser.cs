using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Tweetinvi;

namespace HookHook.Backend.Area
{
    [BsonIgnoreExtraElements]
    public class TwitterFollowUser: IAction, IReaction
    {
        public string UserName {get; set;}

        [BsonIgnore]
        public TwitterClient _twitterClient;

        public List<long> Followers { get; private init; } = new();

        private IConfiguration _config;

        public TwitterFollowUser(string user, IConfiguration config)
        {
            UserName = user;
            _config = config;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _twitterClient = new TwitterClient(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret);

            var userToCheck = await _twitterClient.Users.GetAuthenticatedUserAsync();

            var followers = userToCheck.GetFollowers();

            while (!followers.Completed) {

                var page = await followers.NextPageAsync();

                foreach (var follower in page) {

                    if (Followers.Contains(follower.Id)) {
                        continue;
                    }

                    // todo save
                    Followers.Add(follower.Id);
                    return (follower.Name, true);
                }
            }
            return (null, false);
        }

        public async Task Execute(User user)
        {
            _twitterClient = new TwitterClient(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret);

            var userToFollow = await _twitterClient.UsersV2.GetUserByNameAsync(UserName);

            var currentUser = await _twitterClient.Users.GetAuthenticatedUserAsync();
            await _twitterClient.Users.FollowUserAsync(userToFollow.User.Id);
        }
    }
}
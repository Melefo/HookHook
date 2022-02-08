using CoreTweet;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [BsonIgnoreExtraElements]
    public class TwitterFollowUser: IAction, IReaction
    {
        public string UserName {get; set;}

        [BsonIgnore]
        private Tokens _twitterClient;

        public List<long> Followers { get; private init; } = new();

        private IConfiguration _config;

        public TwitterFollowUser(string user, TwitterService service, IConfiguration config)
        {
            UserName = user;
            _config = config;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret, long.Parse(user.TwitterOAuth.UserId));

            var followers = await _twitterClient.Followers.ListAsync(_twitterClient.UserId);

            foreach (var follower in followers)
            {
                if (!follower.Id.HasValue)
                    continue;
                if (Followers.Contains(follower.Id.Value))
                    continue;

                // todo save
                Followers.Add(follower.Id.Value);
                return (follower.Name, true);
            }
            return (null, false);
        }

        public async Task Execute(User user)
        {
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret, long.Parse(user.TwitterOAuth.UserId));

            var currentUser = await _twitterClient.Users.ShowAsync(_twitterClient.UserId);
            await _twitterClient.Friendships.CreateAsync(UserName);
        }
    }
}
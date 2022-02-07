using CoreTweet;
using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [BsonIgnoreExtraElements]
    public class TwitterTweetHashtag: IAction, IReaction
    {
        public string Hashtag {get; set;}

        public string TweetContent { get; set; }

        [BsonIgnore]
        public Tokens _twitterClient;

        public List<long> Tweets { get; private init; } = new();

        private IConfiguration _config;

        public TwitterTweetHashtag(string hashtag, IConfiguration config, string tweetContent = "")
        {
            Hashtag = hashtag;
            TweetContent = tweetContent;
            _config = config;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret, long.Parse(user.TwitterOAuth.UserId));
            // * https://linvi.github.io/tweetinvi/dist/twitter-api-v1.1/tweets.html
            //var tweets = await _twitterClient.TweetsV2.GetTweetsAsync();


            return (null, false);
        }

        public async Task Execute(User user)
        {
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret, long.Parse(user.TwitterOAuth.UserId));
        }
    }
}
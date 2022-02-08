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

            // * you might want to search for a hashtag, and get the latest one
            // * jsp ce que c'est product et label
            var tweets = await  _twitterClient.Tweets.SearchAsync(product: "", query: Hashtag, label: "");

            foreach (var tweet in tweets) {
                if (Tweets.Contains(tweet.Id))
                    continue;

                // todo save
                Tweets.Add(tweet.Id);
                return (tweet.FullText, true);
            }

            return (null, false);
        }

        public async Task Execute(User user)
        {
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], user.TwitterOAuth.AccessToken, user.TwitterOAuth.OAuthSecret, long.Parse(user.TwitterOAuth.UserId));

            await _twitterClient.Statuses.UpdateAsync(status: $"{TweetContent}\n{Hashtag}");
        }
    }
}
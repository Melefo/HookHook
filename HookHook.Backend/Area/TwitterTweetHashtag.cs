using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [Service("twitter", "Tweet containing an #hashtag")]
    [BsonIgnoreExtraElements]
    public class TwitterTweetHashtag: IAction, IReaction
    {
        public string Hashtag {get; set;}

        public string TweetContent { get; set; }

        [BsonIgnore]
        public Tokens _twitterClient;

        public List<long> Tweets { get; private init; } = new();

        private IConfiguration _config;

        public string _clientId { get; private init; }
        public string _clientSecret { get; private init;}

        public string _serviceAccountId;

        public TwitterTweetHashtag(string hashtag, IConfiguration config, string serviceAccountId, string tweetContent = "")
        {
            Hashtag = hashtag;
            TweetContent = tweetContent;
            _clientId = config["Twitter:ClientId"];
            _clientSecret = config["Twitter:ClientSecret"];
            _config = config;
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == _serviceAccountId);

            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            // * you might want to search for a hashtag, and get the latest one
            // * jsp ce que c'est product et label, et il trouve pas manifestement
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
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == _serviceAccountId);

            Console.WriteLine(oauth);
            Console.WriteLine(oauth.AccessToken);
            Console.WriteLine(oauth.Secret);

            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            await _twitterClient.Statuses.UpdateAsync(status: $"{TweetContent}\n{Hashtag}");
        }
    }
}
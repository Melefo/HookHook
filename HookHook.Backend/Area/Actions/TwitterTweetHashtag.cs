using CoreTweet;
using CoreTweet.V2;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using Tweet = CoreTweet.V2.Tweet;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Twitter user tweet with an #hashtag action
    /// </summary>
    [Service(Providers.Twitter, "Tweet containing an #hashtag")]
    [BsonIgnoreExtraElements]
    public class TwitterTweetHashtag : IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "tweet.text", "tweet.date", "tweet.id", "tweet.source"
        };

        /// <summary>
        /// Hashtag
        /// </summary>
        public string Hashtag { get; set; }
        /// <summary>
        /// Twitter service account id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved tweet with the hastag
        /// </summary>
        public List<long> Tweets { get; private init; } = new();

        /// <summary>
        /// Client used to check on Twitter API
        /// </summary>
        private Tokens? _twitterClient;

        /// <summary>
        /// Twitter client Id
        /// </summary>
        private readonly string _clientId;
        /// <summary>
        /// Twitter client secret
        /// </summary>
        private readonly string _clientSecret;

        /// <summary>
        /// TwitterTweetHashtag constructor for Mongo
        /// </summary>
        /// <param name="clientId">Twitter client Id</param>
        /// <param name="clientSecret">Twitter client secret</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public TwitterTweetHashtag(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// TwitterTweetHashtag constructor
        /// </summary>
        /// <param name="hashtag">Hashtag</param>
        /// <param name="accountId">Twitter service account Id</param>
        /// <param name="user">HookHook user</param>
        /// <param name="clientId">Twitter client Id</param>
        /// <param name="clientSecret">Twitter client Secret</param>
        public TwitterTweetHashtag([ParameterName("Hashtag")] string hashtag, string accountId, User user, string clientId, string clientSecret) : this(clientId, clientSecret)
        {
            if (hashtag[0] != '#')
                hashtag = $"#{hashtag}";
            Hashtag = hashtag;
            AccountId = accountId;

            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;
            var tweets = GetTweets(oauth).GetAwaiter().GetResult();

            foreach (var tweet in tweets)
                Tweets.Add(tweet.Id);
        }

        /// <summary>
        /// Get all twwet containing hashtag
        /// </summary>
        /// <param name="oauth">User twitter oauth account</param>
        /// <returns></returns>
        public async Task<Tweet[]> GetTweets(OAuthAccount oauth)
        {
            _twitterClient ??= Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            var user = await _twitterClient.Users.ShowAsync(_twitterClient.UserId);
            var req = await _twitterClient.SendRequestAsync(MethodType.Get, "https://api.twitter.com/2/tweets/search/recent", new Dictionary<string, string>()
            {
                { "query", $"from:{user.ScreenName} {Hashtag}" }
            });
            var res = await req.Source.Content.ReadFromJsonAsync<RecentSearchResponse>();

            return res?.Data ?? Array.Empty<Tweet>();
        }

        /// <summary>
        /// Check if a new tweet is tweeted with the hashtag
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>List of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;

            var tweets = await GetTweets(oauth);

            foreach (var tweet in tweets)
            {
                if (Tweets.Contains(tweet.Id))
                    continue;
                Tweets.Add(tweet.Id);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], tweet.Text },
                    { Formatters[1], tweet.CreatedAt?.ToString("G") },
                    { Formatters[2], tweet.Id },
                    { Formatters[3], tweet.Source },
                };
                return (formatters, true);
            }

            return (null, false);
        }
    }
}
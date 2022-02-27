using CoreTweet;
using CoreTweet.V2;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using Tweetinvi.Core.Models;
using Tweet = CoreTweet.V2.Tweet;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Twitter, "Tweet containing an #hashtag")]
    [BsonIgnoreExtraElements]
    public class TwitterTweetHashtag : IAction
    {
        public string Hashtag { get; set; }
        public string AccountId { get; set; }

        public string[] Formatters { get; } = new[]
        {
            "tweet.text", "tweet.date", "tweet.id", "tweet.source"
        };
        public List<long> Tweets { get; private init; } = new();

        private Tokens? _twitterClient;

        private readonly string _clientId;
        private readonly string _clientSecret;

        public TwitterTweetHashtag(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

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
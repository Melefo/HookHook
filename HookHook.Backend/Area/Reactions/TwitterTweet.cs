using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Reactions
{
    /// <summary>
    /// Tweet on twitter reaction
    /// </summary>
    [Service(Providers.Twitter, "Tweet a tweet")]
    [BsonIgnoreExtraElements]
    public class TwitterTweet : IReaction
    {
        /// <summary>
        /// Tweet content
        /// </summary>
        public string TweetContent { get; set; }
        /// <summary>
        /// Twitter service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Client used to check on Twitter API
        /// </summary>
        private Tokens? _twitterClient;

        /// <summary>
        /// Twitter client Id
        /// </summary>
        private readonly string _clientId;
        /// <summary>
        /// Twitter client Secret
        /// </summary>
        private readonly string _clientSecret;

        /// <summary>
        /// TwitterTweet constructor for Mongo
        /// </summary>
        /// <param name="clientId">Twitter client Id</param>
        /// <param name="clientSecret">Twitter client secret</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public TwitterTweet(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// TwitterTweet construcotr
        /// </summary>
        /// <param name="tweetContent">Tweet content</param>
        /// <param name="accountId">Twitter service account Id</param>
        /// <param name="clientId">Twitter client Id</param>
        /// <param name="clientSecret">Twitter client secret</param>
        public TwitterTweet([ParameterName("Tweet content")] string tweetContent, string accountId, string clientId, string clientSecret) : this(clientId, clientSecret)
        {
            TweetContent = tweetContent;
            AccountId = accountId;
        }

        /// <summary>
        /// Tweet the tweet
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatter from action</param>
        /// <returns></returns>
        public async Task Execute(User user, Dictionary<string, object?> formatters)
        {
            var content = TweetContent.FormatParam(formatters);
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;

            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            await _twitterClient.Statuses.UpdateAsync(content);
        }
    }
}
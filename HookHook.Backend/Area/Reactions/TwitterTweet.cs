using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Reactions
{
    [Service(Providers.Twitter, "Tweet a tweet")]
    [BsonIgnoreExtraElements]
    public class TwitterTweet : IReaction
    {
        public string TweetContent { get; set; }
        public string AccountId { get; set; }

        private Tokens? _twitterClient;

        private readonly string _clientId;
        private readonly string _clientSecret;

        public TwitterTweet(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public TwitterTweet([ParameterName("Tweet content")] string tweetContent, string accountId, string clientId, string clientSecret) : this(clientId, clientSecret)
        {
            TweetContent = tweetContent;
            AccountId = accountId;
        }

        public async Task Execute(User user, string actionInfo)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;

            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            await _twitterClient.Statuses.UpdateAsync(status: $"{TweetContent}\n{actionInfo}");
        }
    }
}
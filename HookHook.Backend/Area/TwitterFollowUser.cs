using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [Service("twitter", "Follow a twitter account")]
    [BsonIgnoreExtraElements]
    public class TwitterFollowUser: IAction, IReaction
    {
        public string UserName {get; set;}

        [BsonIgnore]
        private Tokens _twitterClient;

        public List<long> Followers { get; private init; } = new();

        private IConfiguration _config;

        private string _serviceAccountId;

        public TwitterFollowUser(string user, TwitterService service, IConfiguration config, string serviceAccountId)
        {
            UserName = user;
            _config = config;
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == _serviceAccountId);
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

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
            var oauth = user.OAuthAccounts[Providers.Twitter];
            _twitterClient = Tokens.Create(_config["Twitter:ClientId"], _config["Twitter:ClientSecret"], oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            var currentUser = await _twitterClient.Users.ShowAsync(_twitterClient.UserId);
            await _twitterClient.Friendships.CreateAsync(UserName);
        }
    }
}
using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [Service(Providers.Twitter, "Follow a twitter account")]
    [BsonIgnoreExtraElements]
    public class TwitterFollowUser: IAction, IReaction
    {
        public string UserName {get; set;}

        [BsonIgnore]
        private Tokens? _twitterClient;

        public List<long> Followers { get; private init; } = new();

        private IConfiguration _config;

        public string _clientId { get; private init; }
        public string _clientSecret { get; private init;}

        public string AccountId { get; set; }

        public TwitterFollowUser([ParameterName("Username")] string user, TwitterService service, IConfiguration config, string accountId, User userEntity)
        {
            UserName = user;
            _clientId = config["Twitter:ClientId"];
            _clientSecret = config["Twitter:ClientSecret"];
            _config = config;
            AccountId = accountId;

            // * save existing followings
            var existingFollowers = GetFollowers(userEntity).GetAwaiter().GetResult();
            foreach (var follower in existingFollowers) {
                if (!follower.Id.HasValue)
                    continue;
                Followers.Add(follower.Id.Value);
            }
        }

        private async Task<Cursored<CoreTweet.User>> GetFollowers(User user)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;
            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            var followers = await _twitterClient.Followers.ListAsync(_twitterClient.UserId);

            return (followers);
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var followers = await GetFollowers(user);

            foreach (var follower in followers)
            {
                if (!follower.Id.HasValue)
                    continue;
                if (Followers.Contains(follower.Id.Value))
                    continue;

                Followers.Add(follower.Id.Value);
                return (follower.Name, true);
            }
            return (null, false);
        }

        public async Task Execute(User user, string actionInfo)
        {
            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;
            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            var currentUser = await _twitterClient.Users.ShowAsync(_twitterClient.UserId);
            await _twitterClient.Friendships.CreateAsync(UserName);
        }
    }
}
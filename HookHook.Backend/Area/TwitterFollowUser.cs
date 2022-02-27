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
        public string Username {get; set;}
        public string AccountId { get; set; }

        public List<long> Followers { get; private init; } = new();

        private Tokens? _twitterClient;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public TwitterFollowUser(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public TwitterFollowUser([ParameterName("Username")] string username, string accountId, User user, string clientId, string clientSecret) : this(clientId, clientSecret)
        {
            Username = username;
            AccountId = accountId;

            var existingFollowers = GetFollowers(user).GetAwaiter().GetResult();

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

            var followers = await _twitterClient.Friends.ListAsync(_twitterClient.UserId, count: 200);
            var list = new List<CoreTweet.User>();

            while (followers.NextCursor != 0)
            {
                foreach (var follower in followers)
                    list.Add(follower);
                followers = await _twitterClient.Friends.ListAsync(_twitterClient.UserId, followers.NextCursor, count: 200);
            }

            foreach (var follower in followers)
                list.Add(follower);

            return followers;
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
            await _twitterClient.Friendships.CreateAsync(Username);
        }
    }
}
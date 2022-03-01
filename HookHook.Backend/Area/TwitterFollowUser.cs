using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    /// <summary>
    /// Follow a twitter account action & reaction
    /// </summary>
    [Service(Providers.Twitter, "Follow a twitter account")]
    [BsonIgnoreExtraElements]
    public class TwitterFollowUser : IAction, IReaction
    {
        /// <summary>
        /// Formatters for reaction
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "following.id", "following.name", "following.username"
        };

        /// <summary>
        /// Twitter username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Twitter service account ID
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of accounts followed saved
        /// </summary>
        public List<long> Followers { get; private init; } = new();

        /// <summary>
        /// Client used to check on Twitter API
        /// </summary>
        private Tokens? _twitterClient;
        /// <summary>
        /// Twitter client ID
        /// </summary>
        private readonly string _clientId;
        /// <summary>
        /// Twitter client Secret
        /// </summary>
        private readonly string _clientSecret;

        /// <summary>
        /// TwitterFollowUser constructor for Mongo
        /// </summary>
        /// <param name="clientId">Twitter client ID</param>
        /// <param name="clientSecret">Twitter client secret</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public TwitterFollowUser(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// TwitterFollowUser constructor
        /// </summary>
        /// <param name="username">Twitter username</param>
        /// <param name="accountId">Twitter service account ID</param>
        /// <param name="user">HookHook user</param>
        /// <param name="clientId">Twitter client ID</param>
        /// <param name="clientSecret">Twitter client Secret</param>
        public TwitterFollowUser([ParameterName("Username")] string username, string accountId, User user, string clientId, string clientSecret) : this(clientId, clientSecret)
        {
            Username = username;
            AccountId = accountId;

            var existingFollowers = GetFollowers(user).GetAwaiter().GetResult();

            foreach (var follower in existingFollowers)
            {
                if (!follower.Id.HasValue)
                    continue;
                Followers.Add(follower.Id.Value);
            }
        }

        /// <summary>
        /// Get the list of all accounts followed by user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of users</returns>
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

        /// <summary>
        /// Check if user followed a new user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>Formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var followers = await GetFollowers(user);

            foreach (var follower in followers)
            {
                if (!follower.Id.HasValue)
                    continue;
                if (Followers.Contains(follower.Id.Value))
                    continue;
                Followers.Add(follower.Id.Value);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], follower.Id },
                    { Formatters[1], follower.Name },
                    { Formatters[2], follower.ScreenName }
                };
                return (formatters, true);
            }
            return (null, false);
        }

        /// <summary>
        /// Follow a new user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatters from reactions</param>
        /// <returns></returns>
        public async Task Execute(User user, Dictionary<string, object?> formatters)
        {
            var username = Username.FormatParam(formatters);

            var oauth = user.ServicesAccounts[Providers.Twitter].SingleOrDefault(acc => acc.UserId == AccountId)!;
            _twitterClient = Tokens.Create(_clientId, _clientSecret, oauth.AccessToken, oauth.Secret, long.Parse(oauth.UserId));

            var currentUser = await _twitterClient.Users.ShowAsync(_twitterClient.UserId);
            await _twitterClient.Friendships.CreateAsync(username);
        }
    }
}
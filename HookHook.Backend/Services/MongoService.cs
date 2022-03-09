using HookHook.Backend.Area;
using HookHook.Backend.Area.Actions;
using HookHook.Backend.Area.Reactions;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// Mongo service database wrapper
    /// </summary>
    public class MongoService
    {
        /// <summary>
        /// Mongo client
        /// </summary>
        private readonly MongoClient _client;
        /// <summary>
        /// Mongo Database
        /// </summary>
        private readonly IMongoDatabase _db;
        /// <summary>
        /// Mongo User Collection
        /// </summary>
        private readonly IMongoCollection<User> _usersCollection;


        /// <summary>
        /// Mongo constructor
        /// </summary>
        /// <param name="config">Host configuration</param>
        public MongoService(GoogleService google, IConfiguration config)
        {
            BsonClassMap.RegisterClassMap<DiscordPinned>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(config["Discord:BotToken"]));
            });
            BsonClassMap.RegisterClassMap<GitHubIssueCreated>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<GitHubNewCommit>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<GitHubNewRepository>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TwitchFollowChannel>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TwitchLiveStarted>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<YoutubeVideoPublished>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(google));
            });
            BsonClassMap.RegisterClassMap<DiscordWebhook>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<GithubCreateIssue>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<GithubCreateRepository>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<YoutubePostComment>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(google));
            });
            BsonClassMap.RegisterClassMap<SpotifyLikeAlbum>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SpotifyLikedAlbum>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SpotifyLikeMusic>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SpotifyLikedMusic>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TwitterTweetHashtag>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(config["Twitter:ClientId"], config["Twitter:ClientSecret"]));
            });
            BsonClassMap.RegisterClassMap<TwitterTweet>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(config["Twitter:ClientId"], config["Twitter:ClientSecret"]));
            });
            BsonClassMap.RegisterClassMap<TwitterFollowUser>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(x => new(config["Twitter:ClientId"], config["Twitter:ClientSecret"]));
            });

            BsonSerializer.RegisterSerializer(new EnumSerializer<Providers>(BsonType.String));
            _client = new MongoClient(config["Mongo:Client"]);
            _db = _client.GetDatabase(config["Mongo:Database"]);

            _usersCollection = _db.GetCollection<User>("Users");
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of user</returns>
        public List<User> GetUsers() =>
            _usersCollection.Find(x => true).ToList();

        /// <summary>
        /// Get an user by its username
        /// </summary>
        /// <param name="username">User's username</param>
        /// <returns>User account</returns>
        public User? GetUser(string id) =>
            _usersCollection.Find(x => x.Id == id).SingleOrDefault();

        /// <summary>
        /// Get user by its authentication informations
        /// </summary>
        /// <param name="identifier">User username or email</param>
        /// <returns>User account</returns>
        public User? GetUserByIdentifier(string identifier) =>
            _usersCollection.Find(x => x.Username == identifier || x.Email == identifier.ToLowerInvariant()).SingleOrDefault();

        public User? GetUserByRandomId(string identifier) =>
            _usersCollection.Find(x => x.RandomId == identifier).SingleOrDefault();

        public User? GetUserByProvider(Providers provider, string id) =>
            _usersCollection.Find(Builders<User>.Filter.Eq($"OAuthAccounts.{provider}.UserId", id)).SingleOrDefault();

        /// <summary>
        /// Create and insert user inside database
        /// </summary>
        /// <param name="u">User accoutn</param>
        public void CreateUser(User u) =>
            _usersCollection.InsertOne(u);

        /// <summary>
        /// Delete an user account inside database
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>true if successful</returns>
        public bool DeleteUser(string id)
        {
            var result = _usersCollection.DeleteOne(x => x.Id == id);

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        /// <summary>
        /// Save an user account inside database
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true if successful</returns>
        public bool SaveUser(User user)
        {
            var result = _usersCollection.ReplaceOne(x => x.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}

using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HookHook.Backend.Attributes;

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
        public MongoService(IConfiguration config)
        {
            // var services = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

            // foreach (var service in services)
            // {
            //     Type serviceType = service.GetType();
            //     TODO BsonClassMap.RegisterClassMap<serviceType>(cm => cm.AutoMap());
            // }

            // * solution temporaire, j'ai cherché je trouve pas comment convertir un type var à un type générique
            BsonClassMap.RegisterClassMap<HookHook.Backend.Reactions.GithubCreateIssue>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<HookHook.Backend.Actions.GithubIssueCreated>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<HookHook.Backend.Actions.GithubNewRepository>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<HookHook.Backend.Reactions.GithubCreateRepository>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<HookHook.Backend.Area.Actions.GithubNewCommit>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<HookHook.Backend.Area.Reactions.DiscordWebhook>(cm => cm.AutoMap());

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
            _usersCollection.Find(x => x.Username == identifier || x.Email == identifier).SingleOrDefault();

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

        public bool SaveUser(User user)
        {
            var result = _usersCollection.ReplaceOne(x => x.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}

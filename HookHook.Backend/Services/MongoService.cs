using HookHook.Backend.Entities;
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
        /// Mongo Area Collection
        /// </summary>
        private readonly IMongoCollection<Area> _areasCollection;
        /// <summary>
        /// Mongo User Areas Collection
        /// </summary>
        private readonly IMongoCollection<UserAreas> _userAreasCollection;

        /// <summary>
        /// Mongo constructor
        /// </summary>
        /// <param name="config">Host configuration</param>
        public MongoService(IConfiguration config)
        {
            _client = new MongoClient(config["Mongo:Client"]);
            _db = _client.GetDatabase(config["Mongo:Database"]);

            _usersCollection = _db.GetCollection<User>("Users");
            _areasCollection = _db.GetCollection<Area>("Areas");
            _userAreasCollection = _db.GetCollection<UserAreas>("UserAreas");
        }
    }
}

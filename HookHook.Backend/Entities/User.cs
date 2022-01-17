using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Entities
{
    /// <summary>
    /// User data saved in database
    /// </summary>
    public class User
    {
        /// <summary>
        /// Database User-Object Id
        /// </summary>
        //[JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private init; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Discord OAuth2
        /// </summary>
        public string DiscordToken { get; set; }
    }
}

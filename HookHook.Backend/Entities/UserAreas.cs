using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Entities
{
    public class UserAreas
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private init; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string Owner { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Areas { get; set; } = new();

        public UserAreas(User user) =>
            Owner = user.Id;
        public UserAreas(string userId) =>
            Owner = userId;
    }
}

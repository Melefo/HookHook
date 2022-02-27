using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Entities
{
    public interface IAction
    {
        public Task<(Dictionary<string, object?>? Result, bool IsSuccess)> Check(User user);

        [BsonIgnore]
        public string[] Formatters { get; }

        public string AccountId { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Entities
{
    public interface IAction
    {
        public Task<(Dictionary<string, object?>? Result, bool IsSuccess)> Check(User user);

        public static string[] Formatters => throw new NotImplementedException();

        public string AccountId { get; set; }
    }
}

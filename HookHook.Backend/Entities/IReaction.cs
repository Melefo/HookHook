namespace HookHook.Backend.Entities
{
    public interface IReaction
    {
        public Task Execute(User user, Dictionary<string, object?> formatters);

        public string AccountId { get; set; }
    }
}

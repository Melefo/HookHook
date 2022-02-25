namespace HookHook.Backend.Entities
{
    public interface IReaction
    {
        public Task Execute(User user, string actionInfo);

        public string AccountId { get; set; }
    }
}

namespace HookHook.Backend.Entities
{
    public interface IAction
    {
        public Task Check(User user, IReaction reaction);
    }
}

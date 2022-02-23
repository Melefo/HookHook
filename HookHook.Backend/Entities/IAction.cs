using HookHook.Backend.Services;

namespace HookHook.Backend.Entities
{
    public interface IAction
    {
        public Task<(string?, bool)> Check(User user);
    }
}

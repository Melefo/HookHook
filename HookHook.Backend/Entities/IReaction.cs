namespace HookHook.Backend.Entities
{
    /// <summary>
    /// Interface used by reactions
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        /// Execute reaction service
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatters from actions</param>
        /// <returns></returns>
        public Task Execute(User user, Dictionary<string, object?> formatters);

        /// <summary>
        /// Service account ID
        /// </summary>
        public string AccountId { get; set; }
    }
}

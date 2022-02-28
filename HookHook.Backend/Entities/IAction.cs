namespace HookHook.Backend.Entities
{
    /// <summary>
    /// Action interface
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Check if action is valid
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>Formatters with service</returns>
        public Task<(Dictionary<string, object?>? Result, bool IsSuccess)> Check(User user);

        /// <summary>
        /// List of formatters used
        /// </summary>
        public static string[] Formatters => throw new NotImplementedException();

        /// <summary>
        /// Service account id
        /// </summary>
        public string AccountId { get; set; }
    }
}

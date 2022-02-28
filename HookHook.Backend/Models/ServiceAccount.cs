namespace HookHook.Backend.Models
{
	/// <summary>
    /// Service account model
    /// </summary>
	public class ServiceAccount
	{
		/// <summary>
        /// Service user ID
        /// </summary>
		public string UserId { get; set; }
		/// <summary>
        /// Service user name
        /// </summary>
		public string Username { get; set; }

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="username">USer name</param>
		public ServiceAccount(string userId, string username)
		{
			UserId = userId;
			Username = username;
		}
	}
}


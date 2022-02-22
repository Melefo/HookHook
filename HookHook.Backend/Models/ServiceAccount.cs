using System;

namespace HookHook.Backend.Models
{
	public class ServiceAccount
	{
		public string UserId { get; set; }
		public string Username { get; set; }

		public ServiceAccount(string userId, string username)
		{
			UserId = userId;
			Username = username;
		}
	}
}


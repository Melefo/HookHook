using System;
using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
	public class PasswordModel
	{
		/// <summary>
		/// User password
		/// </summary>
		[MinLength(4, ErrorMessage = "Password must contain at least 4 characters")]
		[MaxLength(256, ErrorMessage = "Password must be less than 256 characters")]
		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Id is required")]
		public string Id { get; set; }

		public PasswordModel(string id, string password)
		{
			Id = id;
			Password = password;
		}
	}
}


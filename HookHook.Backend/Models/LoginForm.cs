using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Login form data
    /// </summary>
    public class LoginForm
    {
        /// <summary>
        /// User username or email
        /// </summary>
        [MinLength(2, ErrorMessage = "Username must contain at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Username must be less than 256 characters")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [MinLength(4, ErrorMessage = "Password must contain at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Password must be less than 256 characters")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        /// <summary>
        /// LoginForm constructor
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public LoginForm(string username,  string password)
        {
            Username = username;
            Password = password;
        }
    }
}
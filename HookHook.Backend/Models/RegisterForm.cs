using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Register form data
    /// </summary>
    public class RegisterForm
    {
        /// <summary>
        /// User username
        /// </summary>
        [MinLength(2, ErrorMessage = "Username must contain at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Username must be less than 256 characters")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [EmailAddress(ErrorMessage = "Email not valid")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        /// <summary>
        /// User first name
        /// </summary>
        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }

        /// <summary>
        /// User last name
        [Required(ErrorMessage = "Lastname is required")]
        public string LastName { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [MinLength(4, ErrorMessage = "Password must contain at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Password must be less than 256 characters")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        /// <summary>
        /// RegisterForm constructor
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="password">Password</param>
        public RegisterForm(string username, string email, string firstName, string lastName, string password)
        {
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }
    }
}
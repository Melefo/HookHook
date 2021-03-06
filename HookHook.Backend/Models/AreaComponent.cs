using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Area component data
    /// </summary>
    public class AreaComponent
    {
        /// <summary>
        /// Type of component
        /// </summary>
        [Required(ErrorMessage = "Component type is required")]
        public string Type { get; set; }

        /// <summary>
        /// Arguments for component
        /// </summary>
        [Required(ErrorMessage = "Component arguments are required")]
        public string[] Arguments {get; set;}

       /// <summary>
        /// AccountID
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// AreaComponent constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="arguments">Arguments</param>
        /// <param name="accountId">AccountID</param>
        public AreaComponent(string type, string[] arguments, string accountId)
        {
            Type = type;
            Arguments = arguments;
            AccountId = accountId;
        }

    }
}
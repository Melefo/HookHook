using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Action data
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
        public List<string> Arguments = new();

        /// <summary>
        /// AreaComponent constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="arguments">Arguments</param>
        public AreaComponent(string type, List<string> arguments)
        {
            Type = type;
            Arguments = arguments;
        }

    }
}
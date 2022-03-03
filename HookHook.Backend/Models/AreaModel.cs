using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Area data
    /// </summary>
    public class AreaModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        [Required(ErrorMessage = "Action is required")]
        public AreaComponent Action { get; set; }
        /// <summary>
        /// Reactions
        /// </summary>
        [Required(ErrorMessage = "Reactions are required")]
        public AreaComponent[] Reactions { get; set; }

        /// <summary>
        /// Reactions
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Area constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="action">Action</param>
        /// <param name="reactions">Reactions</param>
        /// <param name="minutes">Minutes</param>
        public AreaModel(string name, AreaComponent action, AreaComponent[] reactions, int minutes = 1)
        {
            Name = name;
            Action = action;
            Reactions = reactions;
            Minutes = minutes;
        }

    }
}
using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Area data
    /// </summary>
    public class AreaModel
    {
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
        /// <param name="action">Action</param>
        /// <param name="reactions">Reactions</param>
        public AreaModel(AreaComponent action, AreaComponent[] reactions, int minutes = 1)
        {
            Action = action;
            Reactions = reactions;
            Minutes = minutes;
        }

    }
}
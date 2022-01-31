using System.ComponentModel.DataAnnotations;

namespace HookHook.Backend.Models
{
    /// <summary>
    /// Area data
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Action
        /// </summary>
        [Required(ErrorMessage = "Action is required")]
        public AreaComponent Action {get; set;}
        public List<AreaComponent> Reactions = new();

        /// <summary>
        /// Area constructor
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="reactions">Reactions</param>
        public Area(AreaComponent action, List<AreaComponent> reactions)
        {
            Action = action;
            Reactions = reactions;
        }

    }
}
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Attributes
{
    /// <summary>
    /// Give information to a service by reflection
    /// </summary>
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Type of service
        /// </summary>
        public Providers Name;
        /// <summary>
        /// Description of service
        /// </summary>
        public string Description;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Type</param>
        /// <param name="description">Description</param>
        public ServiceAttribute(Providers name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
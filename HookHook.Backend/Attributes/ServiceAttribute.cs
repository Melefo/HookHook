using HookHook.Backend.Utilities;

namespace HookHook.Backend.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public Providers Name;
        public string Description;

        public ServiceAttribute(Providers name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
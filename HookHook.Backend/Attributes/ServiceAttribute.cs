namespace HookHook.Backend.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public string Name;
        public string Description;

        public ServiceAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
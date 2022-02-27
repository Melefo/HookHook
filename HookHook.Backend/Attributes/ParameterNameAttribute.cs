namespace HookHook.Backend.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class ParameterNameAttribute : Attribute
	{
		public string Name;

        public ParameterNameAttribute(string name) =>
			Name = name;
    }
}


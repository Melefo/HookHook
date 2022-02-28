namespace HookHook.Backend.Attributes
{
	/// <summary>
    /// Attribute used to name action & reaction parameters by reflection
    /// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class ParameterNameAttribute : Attribute
	{
		/// <summary>
        /// Parameter name
        /// </summary>
		public string Name;

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        public ParameterNameAttribute(string name) =>
			Name = name;
    }
}


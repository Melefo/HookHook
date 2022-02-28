namespace HookHook.Backend.Utilities
{
	/// <summary>
    /// Class containing extensions for string class
    /// </summary>
	public static class StringExtensions
	{
		/// <summary>
        /// Format an AREA reaction parameter with action formatters
        /// </summary>
        /// <param name="param">Reaction parameter</param>
        /// <param name="formatters">Action formatters</param>
        /// <returns></returns>
		public static string FormatParam(this string param, Dictionary<string, object?> formatters)
		{
			var formatted = param;

			foreach (var formatter in formatters)
				formatted = formatted.Replace($"{{{formatter.Key}}}", formatter.Value?.ToString());
			return formatted;
		}
	}
}
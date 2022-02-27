using System;

namespace HookHook.Backend.Utilities
{
	public static class StringExtensions
	{
		public static string FormatParam(this string param, Dictionary<string, object?> formatters)
		{
			var formatted = param;

			foreach (var formatter in formatters)
				formatted = formatted.Replace($"{{{formatter.Key}}}", formatter.Value?.ToString());
			return formatted;
		}
	}
}
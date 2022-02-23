using System;
using System.Reflection;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;

namespace HookHook.Backend.Utilities
{
	public enum Providers
	{
		Twitter,
		Twitch,
		Google,
		GitHub,
		Spotify,
		Discord
	}

	public static class InterfaceExtensions
	{
		public static Providers GetProvider(this IAction action)
		{
			var service = action.GetType();
			var attr = service.GetCustomAttribute<ServiceAttribute>();

			return attr!.Name;
		}

		public static Providers GetProvider(this IReaction action)
		{
			var service = action.GetType();
			var attr = service.GetCustomAttribute<ServiceAttribute>();

			return attr!.Name;
		}
	}
}
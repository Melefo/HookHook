using System;
using System.Reflection;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;

namespace HookHook.Backend.Utilities
{
	/// <summary>
    /// List of available services
    /// </summary>
	public enum Providers
	{
		Twitter,
		Twitch,
		Google,
		GitHub,
		Spotify,
		Discord
	}

	/// <summary>
    /// Class contaioning extensions from interfaces
    /// </summary>
	public static class InterfaceExtensions
	{
		/// <summary>
        /// Get service type from an action
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Type of service</returns>
		public static Providers GetProvider(this IAction action)
		{
			var service = action.GetType();
			var attr = service.GetCustomAttribute<ServiceAttribute>();

			return attr!.Name;
		}

		/// <summary>
        /// Get service type from a reaction
        /// </summary>
        /// <param name="reaction">Reaction</param>
        /// <returns>Type of service</returns>
		public static Providers GetProvider(this IReaction reaction)
		{
			var service = reaction.GetType();
			var attr = service.GetCustomAttribute<ServiceAttribute>();

			return attr!.Name;
		}
	}
}
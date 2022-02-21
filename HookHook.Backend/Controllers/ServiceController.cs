using System;
using Discord.Rest;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HookHook.Backend.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class ServiceController : ControllerBase
	{
		private MongoService _mongo;
        private DiscordService _discord;


		public ServiceController(MongoService mongo, DiscordService discord, IConfiguration config)
		{
			_mongo = mongo;
            _discord = discord;
		}

        [HttpGet("{provider}")]
		public async Task<ActionResult<List<ServiceAccount>>> Get(string provider)
        {
			var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
				List<ServiceAccount> list = new();
				foreach (var account in user.DiscordServices ?? new())
                {
                    await _discord.Refresh(account);

                    var client = new DiscordRestClient();
                    await client.LoginAsync(Discord.TokenType.Bearer, account.AccessToken);

                    list.Add(new(account.UserId, client.CurrentUser.ToString()));
                }
                _mongo.SaveUser(user);
				return list;
            }

            return BadRequest();
        }

		[HttpPost("{provider}")]
		public async Task<ActionResult> Add(string provider, string code)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await _discord.AddAccount(user, code);
                    return NoContent();
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
                }
            }
            return BadRequest();
        }

        [HttpDelete("{provider}")]
        public async Task<ActionResult> Delete(string provider, string id)
        {
            var user = _mongo.GetUser(HttpContext.User.Identity!.Name!);

            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
                var service = user.DiscordServices.SingleOrDefault(x => x.UserId == id);

                if (service != null)
                    user.DiscordServices.Remove(service);
                _mongo.SaveUser(user);

                return NoContent();
            }
            return BadRequest();
        }
	}
}


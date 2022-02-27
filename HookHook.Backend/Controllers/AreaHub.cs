using System;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HookHook.Backend.Controllers
{
	[Authorize]
	public class AreaHub : Hub
	{
    }
}


using HookHook.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace HookHook.Backend.Controllers
{
    /// <summary>
    /// /about.json route controller
    /// </summary>
    [ApiController]
    [Route("[controller].json")]
    public class AboutController : ControllerBase
    {
        /// <summary>
        /// GET request on /about.json
        /// </summary>
        /// <returns>About model in json</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<About> Get()
        {
            string clientIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
             
            return new About(clientIp);
        }
    }
}
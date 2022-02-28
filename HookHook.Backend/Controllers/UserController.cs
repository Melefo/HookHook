using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HookHook.Backend.Controllers
{
    /// <summary>
    /// user information/auth route
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly MongoService _db;
        private readonly AreaService _area;

        public UserController(UserService service, MongoService db, AreaService area)
        {
            _service = service;
            _db = db;
            _area = area;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A list of users</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<User>> All() =>
            _service.GetUsers();

        /// <summary>
        /// Delete an user from database
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Request Accepted</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult Delete(string id)
        {
            _service.Delete(id);
            return Accepted();
        }

        /// <summary>
        /// Promote user to Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("promote/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult PromoteUser(string id)
        {
            _service.Promote(id);
            return Accepted();
        }

        /// <summary>
        /// Execute a user's areas
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("trigger/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TriggerUserAreas(string id)
        {
            var user = _db.GetUser(id);
            if (user == null)
                return BadRequest();

            await _area.ExecuteUser(user);
            return Ok();
        }
    }
}
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
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service) =>
            _service = service;

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of User</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<User>> GetUsers() =>
            _service.GetUsers();

        /// <summary>
        /// Register an user to database
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns>return newly created if succesfully registered</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult Create([FromBody] RegisterForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(form);
            var user = new User(form.Username, form.Email, form.FirstName, form.LastName, form.Password);
            if (_service.GetUsers().Count == 0)
                user.Role = "Admin";
            try
            {
                _service.Create(user);
            }
            catch (UserException ex)
            {
                return ex.Type switch
                {
                    TypeUserException.Email => BadRequest(new {errors = new {Email = ex.Message}}),
                    TypeUserException.Password => BadRequest(new {errors = new {Password = ex.Message}}),
                    TypeUserException.Username => BadRequest(new {errors = new {Username = ex.Message}}),
                    _ => BadRequest(new {error = ex.Message})
                };
            }

            return Created("", null);
        }
        
        /// <summary>
        /// Delete an user from database
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Request Accepted</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult Delete([BindRequired] string id)
        {
            _service.Delete(id);
            return Accepted();
        }

        /// <summary>
        /// Login user to API
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Login([FromBody] LoginForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(form);
            try
            {
                var token = _service.Authenticate(form.Username, form.Password);
                return Ok(new { token });
            }
            catch (MongoException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (UserException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Promote user to Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("promote")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult PromoteUser(string id)
        {
            _service.Promote(id);
            return Accepted();
        }

        /// <summary>
        /// Login and link user with google account
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        // [HttpPost("login/google")]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<ActionResult> Google([BindRequired] string code)
        // {
        //     try
        //     {
        //         (var token, var user) = await _service.GoogleAuthenticate(code);

        //         return Ok(new { token, user });
        //     }
        //     catch (ApiException ex)
        //     {
        //         return Unauthorized(new { error = ex.Message });
        //     }
        //     catch (UserException ex)
        //     {
        //         return Unauthorized(new { error = ex.Message });
        //     }
        // }
    }
}
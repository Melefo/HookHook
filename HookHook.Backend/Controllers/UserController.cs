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
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service) =>
            _service = service;

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<User>> All() =>
            _service.GetUsers();

        /// <summary>
        /// Delete an user from database
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Request Accepted</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult Delete(string id)
        {
            _service.Delete(id);
            return Accepted();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("refresh/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult RefreshUser(string id)
        {
            _service.Refresh(id);
            return Accepted();
        }

        /// <summary>
        /// Promote user to Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("promote/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public ActionResult PromoteUser(string id)
        {
            _service.Promote(id);
            return Accepted();
        }

        /// <summary>
        /// Register an user to database
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns>return newly created if succesfully registered</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult Create([FromBody] RegisterForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(form);
            var user = new User(form.Email);
            user.Username = form.Username;
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;
            user.Password = form.Password;
            if (_service.GetUsers().Count == 0)
                user.Role = "Admin";
            try
            {
                _service.Register(user);
            }
            catch (UserException ex)
            {
                return ex.Type switch
                {
                    TypeUserException.Email => BadRequest(new { errors = new { Email = ex.Message } }),
                    TypeUserException.Password => BadRequest(new { errors = new { Password = ex.Message } }),
                    TypeUserException.Username => BadRequest(new { errors = new { Username = ex.Message } }),
                    _ => BadRequest(new { error = ex.Message })
                };
            }

            return Created("", null);
        }


        /// <summary>
        /// Register an user to database with OAuth
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns>return newly created if succesfully registered</returns>
        [HttpPost("oauth/{provider}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> OAuth(string provider, [BindRequired] [FromQuery] string code, [FromQuery] string? verifier)
        {
            if (string.Equals(provider, "Discord", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    string token = await _service.DiscordOAuth(code, HttpContext);

                    return Ok(new {token});
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message});
                }
            }

            if (string.Equals(provider, "Spotify", StringComparison.InvariantCultureIgnoreCase)) {
                try {
                    string token = await _service.SpotifyOAuth(code, HttpContext);

                    return Ok(new {token});
                } catch (ApiException ex) {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message});
                }
            }

            if (string.Equals(provider, "Twitch", StringComparison.InvariantCultureIgnoreCase)) {
                try {
                    string token = await _service.TwitchOAuth(code, HttpContext);

                    return (Ok(new {token}));
                } catch (ApiException e) {
                    return (StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = e.Message}));
                }
            }
            if (string.Equals(provider, "GitHub", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    string token = await _service.GitHubOAuth(code, HttpContext);

                    return Ok(new {token});
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message});
                }
            }
            if (string.Equals(provider, "Twitter", StringComparison.InvariantCultureIgnoreCase) && verifier != null) {
                try {
                    string token = await _service.TwitterOAuth(code, verifier!, HttpContext);

                    return Ok(new {token});
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message});
                }
            }
            if (string.Equals(provider, "Google", StringComparison.InvariantCultureIgnoreCase)) {
                try {
                    string token = await _service.GoogleOAuth(code, HttpContext);

                    return Ok(new {token});
                }
                catch (ApiException ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new {error = ex.Message});
                }
            }

            return BadRequest();
        }

        [HttpGet("authorize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Authorize([BindRequired] [FromQuery] string provider)
        {
            if (!string.Equals(provider, "Twitter", StringComparison.InvariantCultureIgnoreCase))
                return BadRequest();
            return _service.TwitterAuthorize();
        }

        /// <summary>
        /// Login user to API
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns></returns>
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
    }
}
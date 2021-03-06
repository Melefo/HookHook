using System.Net;
using System.Net.Mail;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Models;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HookHook.Backend.Controllers
{
    /// <summary>
    /// /signin controller route
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SignInController : ControllerBase
    {
        private readonly UserService _service;
        private readonly IConfiguration _config;

        public SignInController(UserService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        /// <summary>
        /// Register an user to database
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns>return newly created if succesfully registered</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> Create([FromBody] RegisterForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(form);
            User user = new(form);
            if (_service.GetUsers().Count == 0)
                user.Role = "Admin";
            try
            {
                user = _service.Register(user);
                string html = $@"
<html>
    <body>
        <h1>Welcome to HookHook!<h1>
        <p>Please <a href=""{Request.Headers.Origin}/verify/{user.RandomId}"">click here</a> to confirm your registration.</p>
    </bod>
</html>";
                await _service.SendMail(user.Email, "Welcome to HookHook!", html);

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> OAuth(Providers provider, [BindRequired][FromQuery] string code, [FromQuery] string? verifier = null, [FromQuery] string? redirect = null)
        {
            try
            {
                string token = provider switch
                {
                    Providers.Discord => await _service.DiscordOAuth(code, verifier, redirect!, HttpContext),
                    Providers.Spotify => await _service.SpotifyOAuth(code, redirect!, HttpContext),
                    Providers.Twitch => await _service.TwitchOAuth(code, HttpContext),
                    Providers.GitHub => await _service.GitHubOAuth(code, HttpContext),
                    Providers.Twitter => await _service.TwitterOAuth(code, verifier!, HttpContext),
                    Providers.Google => await _service.GoogleOAuth(code, HttpContext),
                    _ => throw new ArgumentException("Unknown type", nameof(provider))
                };
                return Ok(new { token });

            }
            catch (ApiException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get twitter authorization
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Verifier code</returns>
        [HttpGet("authorize/{provider}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Authorize(Providers provider, [FromQuery] [BindRequired] string redirect)
        {
            if (provider != Providers.Twitter)
                return BadRequest();
            return _service.TwitterAuthorize(redirect);
        }

        /// <summary>
        /// Login user to API
        /// </summary>
        /// <param name="form">User informations</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// Verify an ID
        /// </summary>
        /// <param name="id"></param>
        [HttpPut("verify/{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Verify([BindRequired] string id)
        {
            try
            {
                var token = _service.Verify(id);
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
        /// Recover password
        /// </summary>
        /// <param name="username"></param>
        [HttpPut("forgot/{username}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ForgotPassword(string username)
        {
            await _service.RecoverPassword(username, Request.Headers.Origin);
            return NoContent();
        }

        /// <summary>
        /// Confirm password
        /// </summary>
        /// <param name="form"></param>
        [HttpPut("confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ConfirmPassword([FromBody] PasswordModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(form);
            try
            {
                var token = _service.ConfirmPassword(form.Id, form.Password);
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
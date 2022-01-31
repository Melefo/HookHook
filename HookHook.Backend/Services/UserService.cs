using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Discord;
using Discord.Rest;
using Octokit;
using ApiException = HookHook.Backend.Exceptions.ApiException;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// User realted service
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Database access
        /// </summary>
        private readonly MongoService _db;

        /// <summary>
        /// JWT key
        /// </summary>
        private readonly string _key;

        private readonly string _discordId;
        private readonly string _discordSecret;
        private readonly string _discordRedirect;

        private readonly string _gitHubId;
        private readonly string _gitHubSecret;

        private readonly HttpClient _client = new();

        public UserService(MongoService db, IConfiguration config)
        {
            _db = db;
            _key = config["JwtKey"];

            _discordId = config["Discord:ClientId"];
            _discordSecret = config["Discord:ClientSecret"];
            _discordRedirect = config["Discord:Redirect"];

            _gitHubId = config["GitHub:ClientId"];
            _gitHubSecret = config["GitHub:ClientSecret"];
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of User</returns>
        public List<User> GetUsers() => _db.GetUsers();

        /// <summary>
        /// Create an User account
        /// </summary>
        /// <param name="user">User informations</param>
        public void Create(User user)
        {
            if (_db.GetUserByIdentifier(user.Email) != null)
                throw new UserException(TypeUserException.Email, "An user with this email is already registered");
            if (user.Username != null && _db.GetUserByIdentifier(user.Username) != null)
                throw new UserException(TypeUserException.Username, "An user with this username is already registered");
            if (user.Password != null)
                user.Password = PasswordHash.HashPassword(user.Password);
            _db.CreateUser(user);
        }

        public void Register(User user)
        {
            var existing = _db.GetUserByIdentifier(user.Email);

            if (existing != null && existing.Password == null)
            {
                if (_db.GetUserByIdentifier(user.Username!) != null)
                    throw new UserException(TypeUserException.Username, "An user with this username is already registered");
                existing.Username = user.Username;
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.Password = PasswordHash.HashPassword(user.Password!);
                _db.SaveUser(existing);
                return;
            }
            Create(user);
        }

        /// <summary>
        /// Authenticate User and give him a token
        /// </summary>
        /// <param name="username">User username or email</param>
        /// <param name="password">User password</param>
        /// <returns>JWT</returns>
        public string Authenticate(string username, string password)
        {
            var user = _db.GetUserByIdentifier(username);

            if (user == null)
                throw new MongoException("User not found");
            if (user.Password == null)
                throw new MongoException("User not found");
            if (!PasswordHash.VerifyPassword(password, user.Password))
                throw new UserException(TypeUserException.Password, "Wrong password");

            return CreateJwt(user);
        }

        public string CreateJwt(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            var tokenKey = Encoding.UTF8.GetBytes(_key);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new(new Claim[]
                {
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role),
                    new(ClaimTypes.Name, user.Id),
                    new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private class DiscordToken
        {
            [JsonPropertyName("access_token")] public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

            [JsonPropertyName("scope")] public string Scope { get; set; }

            [JsonPropertyName("token_type")] public string TokenType { get; set; }
        }

        public async Task<string> DiscordOAuth(string code, HttpContext ctx)
        {
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _discordId),
                new("client_secret", _discordSecret),
                new("grant_type", "authorization_code"),
                new("code", code),
                new("redirect_uri", _discordRedirect),
            });
            var res = await _client.PostAsync<DiscordToken>("https://discord.com/api/oauth2/token", content);

            if (res == null)
                throw new ApiException("Failed to call API");
            var client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bearer, res.AccessToken);

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??= _db.GetUserByDiscord(client.CurrentUser.Id.ToString());
            user ??= _db.GetUserByIdentifier(client.CurrentUser.Email);
            if (user == null)
            {
                user = new(client.CurrentUser.Email);
                Create(user);
            }

            user.Discord = new(client.CurrentUser.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn),
                res.RefreshToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        public async Task<string> GitHubOAuth(string code, HttpContext ctx)
        {
            var client = new GitHubClient(new ProductHeaderValue("HookHook"));

            var request = new OauthTokenRequest(_gitHubId, _gitHubSecret, code);
            var res = await client.Oauth.CreateAccessToken(request);

            if (res == null)
                throw new ApiException("Failed to call API");

            client.Credentials = new Credentials(res.AccessToken);
            var github = await client.User.Current();
            var emails = await client.User.Email.GetAll();
            var email = emails.SingleOrDefault(x => x.Primary);

            User? user = null;
            if (ctx.User.Identity is {IsAuthenticated: true, Name: { }})
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??=  _db.GetUserByGitHub(github.Id.ToString());
            if (email != null)
                user ??= _db.GetUserByIdentifier(email.Email);
            if (user == null)
            {
                user = new(github.Email);
                Create(user);
            }

            user.GitHub = new(github.Id.ToString(), res.AccessToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }
    }
}
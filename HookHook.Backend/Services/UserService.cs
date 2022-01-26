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
            if (_db.GetUserByIdentifier(user.Username) != null)
                throw new UserException(TypeUserException.Username, "An user with this username is already registered");
            user.Password = PasswordHash.HashPassword(user.Password);
            _db.CreateUser(user);
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
            if (!PasswordHash.VerifyPassword(password, user.Password))
                throw new UserException(TypeUserException.Password, "Wrong password");

            return CreateJWT(user);
        }

        public string CreateJWT(User user)
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

        public async Task<(bool, string?)> DiscordOAuth(string code)
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
            var user = _db.GetUserByIdentifier(client.CurrentUser.Email);
            if (user == null)
            {
                Console.WriteLine("nia");
                return (false, null);
            }

            user.DiscordToken = res.RefreshToken;
            _db.SaveUser(user);

            return (true, CreateJWT(user));
        }

        public async Task<(bool, string?)> GitHubOAuth(string code)
        {
            var client = new GitHubClient(new ProductHeaderValue("HookHook"));

            var request = new OauthTokenRequest(_gitHubId, _gitHubSecret, code);
            var res = await client.Oauth.CreateAccessToken(request);

            if (res == null)
                throw new ApiException("Failed to call API");

            client.Credentials = new Credentials(res.AccessToken);

            var emails = await client.User.Email.GetAll();
            var primary = emails.SingleOrDefault(x => x.Primary);

            if (primary == null)
            {
                return (false, null);
            }

            var user = _db.GetUserByIdentifier(primary.Email);
            if (user == null)
            {
                Console.WriteLine("nia");
                return (false, null);
            }

            user.GithubToken = res.AccessToken;
            _db.SaveUser(user);

            return (true, CreateJWT(user));
        }
    }
}
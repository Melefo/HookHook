using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Discord.Rest;
using Octokit;
using SpotifyAPI.Web;
using ApiException = HookHook.Backend.Exceptions.ApiException;
using User = HookHook.Backend.Entities.User;
using HookHook.Backend.Entities;
using CoreTweet;

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
        private readonly TwitterService _twitter;
        private readonly DiscordService _discord;
        private readonly TwitchService _twitch;
        private readonly SpotifyService _spotify;

        /// <summary>
        /// JWT key
        /// </summary>
        private readonly string _key;

        private readonly string _gitHubId;
        private readonly string _gitHubSecret;

        private readonly string _googleId;
        private readonly string _googleSecret;

        private readonly string _discordRedirect = "http://localhost/oauth";

        private readonly HttpClient _client = new();

        public UserService(MongoService db, TwitterService twitter, DiscordService discord, TwitchService twitch, SpotifyService spotify, IConfiguration config)
        {
            _db = db;
            _twitter = twitter;
            _discord = discord;
            _twitch = twitch;
            _spotify = spotify;

            _key = config["JwtKey"];

            _gitHubId = config["GitHub:ClientId"];
            _gitHubSecret = config["GitHub:ClientSecret"];

            _googleId = config["Google:ClientId"];
            _googleSecret = config["Google:ClientSecret"];
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

        public void Delete(string id) =>
            _db.DeleteUser(id);

        /// <summary>
        /// Promote an User to Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Promote(string id)
        {
            User user = _db.GetUser(id);
            user.Role = user.Role == "Admin" ? "User" : "Admin";

            return _db.SaveUser(user);
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

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.Name, user.Id),
            };
            if (user.Email != null)
                claims.Add(new(ClaimTypes.Email, user.Email));
            if (user.FirstName != null && user.LastName != null)
                claims.Add(new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"));
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new(claims),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??= _db.GetUserByGitHub(github.Id.ToString());
            if (email != null)
                user ??= _db.GetUserByIdentifier(email.Email);
            if (user == null)
            {
                user = new(github.Email);
                Create(user);
            }

            user.GitHubOAuth = new(github.Id.ToString(), res.AccessToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        private class GoogleAuth
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonPropertyName("expire_in")]
            public int ExpiresIn { get; set; }
            public string Scope { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; }
        }

        private class GoogleProfile
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }
        }


        public async Task<string> GoogleOAuth(string code, HttpContext ctx)
        {
            var res = await _client.PostAsync<GoogleAuth>($"https://oauth2.googleapis.com/token?code={code}&client_id={_googleId}&client_secret={_googleSecret}&redirect_uri=http://localhost/oauth&grant_type=authorization_code");

            if (res == null)
                throw new ApiException("Failed to call API");

            Console.WriteLine("Successfully called google api");

            // * get user info by decoding the jwt
            JwtSecurityTokenHandler tokenHandler = new();
            string? id = tokenHandler.ReadJwtToken(res.IdToken).Payload["sub"].ToString();
            string? email = tokenHandler.ReadJwtToken(res.IdToken).Payload["email"].ToString();

            if (id == null || email == null)
                throw new ApiException("API did not return the necessary arguments");

            User? user = null;
            if (ctx.User.Identity is {IsAuthenticated: true, Name: { }})
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??=  _db.GetUserByGoogle(id);
            user ??=  _db.GetUserByIdentifier(email);
            if (user == null) {
                user = new(email);
                Create(user);
            }

            user.GoogleOAuth = new(id, res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);
            _db.SaveUser(user);
            return CreateJwt(user);
        }

        public async Task<string> DiscordOAuth(string code, HttpContext ctx)
        {
            (DiscordRestClient client, DiscordToken res) = await _discord.OAuth(code);

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

            user.DiscordOAuth = new(client.CurrentUser.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn),
                res.RefreshToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        public string TwitterAuthorize() =>
            _twitter.Authorize();

        public async Task<string> TwitterOAuth(string code, string verifier, HttpContext ctx)
        {
            (UserResponse twitter, Tokens tokens) = await _twitter.OAuth(code, verifier);

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??= _db.GetUserByTwitter(tokens.UserId.ToString());
            user ??= _db.GetUserByIdentifier(twitter.Email);
            if (user == null)
            {
                user = new(twitter.Email);
                Create(user);
            }

            user.TwitterOAuth = new(tokens.UserId.ToString(), tokens.AccessToken, tokens.AccessTokenSecret);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        public async Task<string> TwitchOAuth(string code, HttpContext ctx)
        {
            (var client, TwitchToken res) = await _twitch.OAuth(code);

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);
            user ??= _db.GetUserByTwitch(client.Id.ToString());
            user ??= _db.GetUserByIdentifier(client.Email);
            if (user == null)
            {
                user = new(client.Email);
                Create(user);
            }

            user.TwitchOAuth = new(client.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn),
                res.RefreshToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        public async Task<string> SpotifyOAuth(string code, HttpContext ctx)
        {
            (var spotify, var response) = await _spotify.OAuth(code);

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);

            var spotifyUser = await spotify.UserProfile.Current();
            user ??= _db.GetUserBySpotify(spotifyUser.Id);
            user ??= _db.GetUserByIdentifier(spotifyUser.Email);
            if (user == null)
            {
                user = new(spotifyUser.Email);
                Create(user);
            }

            user.SpotifyOAuth = new(spotifyUser.Id, response.AccessToken, TimeSpan.FromSeconds(response.ExpiresIn),
                response.RefreshToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }
    }
}
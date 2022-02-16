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
using SpotifyAPI.Web;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Api.Helix.Models.Users;
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
        private readonly TwitterService _twitter;

        /// <summary>
        /// JWT key
        /// </summary>
        private readonly string _key;

        private readonly string _discordId;
        private readonly string _discordSecret;
        private readonly string _discordRedirect;

        private readonly string _gitHubId;
        private readonly string _gitHubSecret;

        private readonly string _spotifyId;
        private readonly string _spotifySecret;

        private readonly string _twitchId;
        private readonly string _twitchSecret;

        private readonly HttpClient _client = new();

        public UserService(MongoService db, TwitterService twitter, IConfiguration config)
        {
            _db = db;
            _twitter = twitter;
            _key = config["JwtKey"];

            _discordId = config["Discord:ClientId"];
            _discordSecret = config["Discord:ClientSecret"];
            _discordRedirect = config["Discord:Redirect"];

            _gitHubId = config["GitHub:ClientId"];
            _gitHubSecret = config["GitHub:ClientSecret"];

            _spotifyId = config["Spotify:ClientId"];
            _spotifySecret = config["Spotify:ClientSecret"];

            _twitchId = config["Twitch:ClientId"];
            _twitchSecret = config["Twitch:ClientSecret"];
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

        private class DiscordToken
        {
            [JsonPropertyName("access_token")] public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

            [JsonPropertyName("scope")] public string Scope { get; set; }

            [JsonPropertyName("token_type")] public string TokenType { get; set; }
        }

        // * exact same as the discordtoken except scope is an array
        private class TwitchToken
        {
           [JsonPropertyName("access_token")] public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

            [JsonPropertyName("scope")] public string[] Scope { get; set; }

            [JsonPropertyName("token_type")] public string TokenType { get; set; }
        }

        public async Task<string> TwitchOAuth(string code, HttpContext ctx)
        {
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _twitchId),
                new("client_secret", _twitchSecret),
                new("code", code),
                new("grant_type", "authorization_code"),
                new("redirect_uri", _discordRedirect),
            });
            var res = await _client.PostAsync<TwitchToken>("https://id.twitch.tv/oauth2/token", content);

            if (res == null)
                throw new ApiException("Failed to call API");

            var api = new TwitchAPI();
            api.Settings.ClientId = _twitchId;
            api.Settings.AccessToken = res.AccessToken;

            var users = await api.Helix.Users.GetUsersAsync(null, null, res.AccessToken);

            if (users == null)
                throw new ApiException("Failed to call API");

            var client = users.Users[0];

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
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(_spotifyId, _spotifySecret, code, new Uri(_discordRedirect))
            );

            if (response == null)
                throw new ApiException("Failed to call API");
            var spotify = new SpotifyClient(response.AccessToken);

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);

            var spotifyUser = await spotify.UserProfile.Current();
            user ??= _db.GetUserBySpotify(spotifyUser.Id.ToString());
            user ??= _db.GetUserByIdentifier(spotifyUser.Email);
            if (user == null) {
                user = new(spotifyUser.Email);
                Create(user);
            }

            user.SpotifyOAuth = new(spotifyUser.Id.ToString(), response.AccessToken, TimeSpan.FromSeconds(response.ExpiresIn),
                response.RefreshToken);
            _db.SaveUser(user);

            return CreateJwt(user);
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

            user.DiscordOAuth = new(client.CurrentUser.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn),
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

            user.GitHubOAuth = new(github.Id.ToString(), res.AccessToken);
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        public string TwitterAuthorize() =>
            _twitter.Authorize();

        public async Task<string> TwitterOAuth(string code, string verifier, HttpContext ctx)
        {
            var tokens = await _twitter.Token(code, verifier);

            if (tokens == null)
                throw new ApiException("Failed to call API");

            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);
            var twitter = await tokens.Users.ShowAsync(tokens.UserId);
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
    }
}
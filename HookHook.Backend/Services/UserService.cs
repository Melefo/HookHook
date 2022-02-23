using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Discord.Rest;
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
        private readonly GitHubService _github;
        private readonly GoogleService _google;

        /// <summary>
        /// JWT key
        /// </summary>
        private readonly string _key;

        public UserService(MongoService db, TwitterService twitter, DiscordService discord, TwitchService twitch, SpotifyService spotify, GitHubService github, GoogleService google, IConfiguration config)
        {
            _db = db;
            _twitter = twitter;
            _discord = discord;
            _twitch = twitch;
            _spotify = spotify;
            _github = github;
            _google = google;

            _key = config["JwtKey"];
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
            User user = _db.GetUser(id)!;
            user.Role = user.Role == "Admin" ? "User" : "Admin";

            return _db.SaveUser(user);
        }

        public void Register(User user)
        {
            var existing = _db.GetUserByIdentifier(user.Email);

            if (existing == null || existing.Password != null)
            {
                Create(user);
                return;
            }
            if (_db.GetUserByIdentifier(user.Username!) != null)
                throw new UserException(TypeUserException.Username, "An user with this username is already registered");
            existing.Username = user.Username;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Password = PasswordHash.HashPassword(user.Password!);
            _db.SaveUser(existing);
            return;
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

        public async Task<string> GoogleOAuth(string code, HttpContext ctx)
        {
            (var profile, var res) = await _google.OAuth(code);
            OAuthAccount account = new(profile.Id, res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Google, profile.Email, account);
        }

        public async Task<string> DiscordOAuth(string code, HttpContext ctx)
        {
            (DiscordRestClient client, DiscordToken res) = await _discord.OAuth(code);
            OAuthAccount account = new(client.CurrentUser.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Discord, client.CurrentUser.Email, account);
        }

        public string TwitterAuthorize() =>
            _twitter.Authorize();

        public async Task<string> TwitterOAuth(string code, string verifier, HttpContext ctx)
        {
            (UserResponse twitter, Tokens tokens) = await _twitter.OAuth(code, verifier);
            OAuthAccount account = new(tokens.UserId.ToString(), tokens.AccessToken, secret: tokens.AccessTokenSecret);

            return OAuth(ctx, Providers.Twitch, twitter.Email, account);
        }

        public async Task<string> TwitchOAuth(string code, HttpContext ctx)
        {
            (var client, TwitchToken res) = await _twitch.OAuth(code);
            OAuthAccount account = new(client.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Twitch, client.Email, account);
        }

        public async Task<string> SpotifyOAuth(string code, HttpContext ctx)
        {
            (var spotify, var res) = await _spotify.OAuth(code);
            var spotifyUser = await spotify.UserProfile.Current();
            OAuthAccount account = new(spotifyUser.Id, res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Spotify, spotifyUser.Email, account);
        }

        public async Task<string> GitHubOAuth(string code, HttpContext ctx)
        {
            (var client, var res) = await _github.OAuth(code);
            var user = await client.User.Current();

            var emails = await client.User.Email.GetAll();
            var primary = emails.SingleOrDefault(x => x.Primary);
            OAuthAccount account = new(user.Id.ToString(), res.AccessToken);

            return OAuth(ctx, Providers.GitHub, primary!.Email, account);
        }

        private string OAuth(HttpContext ctx, Providers provider, string email, OAuthAccount account)
        {
            User? user = null;
            if (ctx.User.Identity is { IsAuthenticated: true, Name: { } })
                user = _db.GetUser(ctx.User.Identity.Name);

            user ??= _db.GetUserByProvider(Providers.Spotify, account.UserId);
            user ??= _db.GetUserByIdentifier(email);
            if (user == null)
            {
                user = new(email);
                Create(user);
            }

            user.OAuthAccounts[provider] = account;
            _db.SaveUser(user);

            return CreateJwt(user);
        }
    }
}
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
using System.Net.Mail;
using System.Net;

namespace HookHook.Backend.Services
{
    /// <summary>
    /// User related service
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Database access
        /// </summary>
        private readonly MongoService _db;
        /// <summary>
        /// Twitter service
        /// </summary>
        private readonly TwitterService _twitter;
        /// <summary>
        /// Discord service
        /// </summary>
        private readonly DiscordService _discord;
        /// <summary>
        /// Twitch service
        /// </summary>
        private readonly TwitchService _twitch;
        /// <summary>
        /// Spotify service
        /// </summary>
        private readonly SpotifyService _spotify;
        /// <summary>
        /// Github service
        /// </summary>
        private readonly GitHubService _github;
        /// <summary>
        /// Google service
        /// </summary>
        private readonly GoogleService _google;

        /// <summary>
        /// JWT key
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// SMTP server
        /// </summary>
        private readonly SmtpClient _smtp;
        /// <summary>
        /// MailAddress
        /// </summary>
        private readonly MailAddress _from;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db">Database</param>
        /// <param name="twitter">Service</param>
        /// <param name="discord">Service</param>
        /// <param name="twitch">Service</param>
        /// <param name="spotify">Service</param>
        /// <param name="github">Service</param>
        /// <param name="google">Service</param>
        /// <param name="config">Environment variables</param>
        public UserService(MongoService db, TwitterService twitter, DiscordService discord, TwitchService twitch, SpotifyService spotify, GitHubService github, GoogleService google, IConfiguration config)
        {
            _db = db;
            _twitter = twitter;
            _discord = discord;
            _twitch = twitch;
            _spotify = spotify;
            _github = github;
            _google = google;

            _smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(config["SMTP:email"], config["SMTP:password"])
            };
            _from = new MailAddress(config["SMTP:Email"], "HookHook");

            _key = config["JwtKey"];
        }

        /// <summary>
        /// Send confirmation mail
        /// </summary>
        /// <param name="To"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        public async Task SendMail(string to, string subject, string body)
        {
            var mail = new MailMessage(_from, new(to))
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };
            await _smtp.SendMailAsync(mail);
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

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id) =>
            _db.DeleteUser(id);

        /// <summary>
        /// Promote an User to Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True/False</returns>
        public bool Promote(string id)
        {
            User user = _db.GetUser(id)!;
            user.Role = user.Role == "Admin" ? "User" : "Admin";

            return _db.SaveUser(user);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The new user</returns>
        public User Register(User user)
        {
            var existing = _db.GetUserByIdentifier(user.Email);

            if (existing == null || existing.Password != null)
            {
                Create(user);
                return user;
            }
            if (_db.GetUserByIdentifier(user.Username!) != null)
                throw new UserException(TypeUserException.Username, "An user with this username is already registered");
            existing.Username = user.Username;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Password = PasswordHash.HashPassword(user.Password!);
            _db.SaveUser(existing);
            return existing;
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
            if (user.Verified != true)
                throw new UserException(TypeUserException.Email, "Please verify your email before logging in.");
            if (!PasswordHash.VerifyPassword(password, user.Password))
                throw new UserException(TypeUserException.Password, "Wrong password");

            return CreateJwt(user);
        }

        /// <summary>
        /// Verify user after registration
        /// </summary>
        /// <param name="id">User username or email</param>
        /// <returns>JWT</returns>
        public string Verify(string id)
        {
            var user = _db.GetUserByRandomId(id);

            if (user == null)
                throw new MongoException("User not found");
            if (user.Verified)
                throw new UserException(TypeUserException.Email, "Email already verified");

            user.Verified = true;
            user.GenerateRandomId();
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        /// <summary>
        /// Recover a user's password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="origin"></param>
        public async Task RecoverPassword(string username, string origin)
        {
            var user = _db.GetUserByIdentifier(username);
            if (user == null)
                return;
            if (user.Email == null)
                return;
            user.GenerateRandomId();
            _db.SaveUser(user);
            var html = $@"
<html>
    <body>
        <h1>Welcome back to HookHook!</h1>
        <p>Please <a href=""{origin}/confirm/{user.RandomId}"">click here</a> to reset your password.</p>
        <p>⚠️ If you didn't ask to reset your password, delete this mail.</p>
    </body>
</html>";
            await SendMail(user.Email, "Recover your password", html);
        }

        /// <summary>
        /// Confirm and cross-check a password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns>JWT</returns>
        public string ConfirmPassword(string id, string password)
        {
            var user = _db.GetUserByRandomId(id);
            if (user == null)
                throw new MongoException("User not found");
            if (user.Email == null)
                throw new UserException(TypeUserException.Email, "Only users with email can reset their password");

            user.Password = PasswordHash.HashPassword(password);
            user.Verified = true;
            user.GenerateRandomId();
            _db.SaveUser(user);

            return CreateJwt(user);
        }

        /// <summary>
        /// Confirm and cross-check a password
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JWT</returns>
        public string CreateJwt(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            var tokenKey = Encoding.UTF8.GetBytes(_key);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.Name, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id)
            };
            if (user.Email != null)
                claims.Add(new(ClaimTypes.Email, user.Email));
            if (user.FirstName != null && user.LastName != null)
                claims.Add(new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"));
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new(claims),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// OAuth with google
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> GoogleOAuth(string code, HttpContext ctx)
        {
            (var profile, var res) = await _google.OAuth(code);
            OAuthAccount account = new(profile.Id, res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Google, profile.Email, account);
        }

        /// <summary>
        /// OAuth with discord
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> DiscordOAuth(string code, HttpContext ctx)
        {
            (DiscordRestClient client, DiscordToken res) = await _discord.OAuth(code);
            OAuthAccount account = new(client.CurrentUser.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Discord, client.CurrentUser.Email, account);
        }

        /// <summary>
        /// Twitter authorize
        /// </summary>
        /// <returns>Authorization code</returns>
        public string TwitterAuthorize() =>
            _twitter.Authorize();

        /// <summary>
        /// OAuth with twitter
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="verifier">Verifier code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> TwitterOAuth(string code, string verifier, HttpContext ctx)
        {
            (UserResponse twitter, Tokens tokens) = await _twitter.OAuth(code, verifier);
            OAuthAccount account = new(tokens.UserId.ToString(), tokens.AccessToken, secret: tokens.AccessTokenSecret);

            return OAuth(ctx, Providers.Twitch, twitter.Email, account);
        }

        /// <summary>
        /// OAuth with twitch
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> TwitchOAuth(string code, HttpContext ctx)
        {
            (var client, TwitchToken res) = await _twitch.OAuth(code);
            OAuthAccount account = new(client.Id.ToString(), res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Twitch, client.Email, account);
        }

        /// <summary>
        /// OAuth with spotify
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> SpotifyOAuth(string code, string redirect, HttpContext ctx)
        {
            (var spotify, var res) = await _spotify.OAuth(code, redirect);
            var spotifyUser = await spotify.UserProfile.Current();
            OAuthAccount account = new(spotifyUser.Id, res.AccessToken, TimeSpan.FromSeconds(res.ExpiresIn), res.RefreshToken);

            return OAuth(ctx, Providers.Spotify, spotifyUser.Email, account);
        }

        /// <summary>
        /// OAuth with github
        /// </summary>
        /// <param name="code">Auth code</param>
        /// <param name="ctx"></param>
        /// <returns>JWT</returns>
        public async Task<string> GitHubOAuth(string code, HttpContext ctx)
        {
            (var client, var res) = await _github.OAuth(code);
            var user = await client.User.Current();

            var emails = await client.User.Email.GetAll();
            var primary = emails.SingleOrDefault(x => x.Primary);
            OAuthAccount account = new(user.Id.ToString(), res.AccessToken);

            return OAuth(ctx, Providers.GitHub, primary!.Email, account);
        }

        /// <summary>
        /// OAuth with provider
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="provider"></param>
        /// <param name="email"></param>
        /// <param name="account"></param>
        /// <returns>JWT</returns>
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
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

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
        // private readonly string _googleId;
        // private readonly string _googleSecret;
        private readonly HttpClient _client = new();

        public UserService(MongoService db, IConfiguration config)
        {
            _db = db;
            _key = config["JwtKey"];
            // _googleId = config["Google:ClientId"];
            // _googleSecret = config["Google:ClientSecret"];
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
                throw new UserException("An user with this email is already registered");
            if (_db.GetUserByIdentifier(user.Username) != null)
                throw new UserException("An user with this username is already registered");
            user.Password = PasswordHash.HashPassword(user.Password);
            _db.CreateUser(user);
        }

        /// <summary>
        /// Deleta an User account
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>If successfully deleted</returns>
        public bool Delete(string id) => _db.DeleteUser(id);

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

        // private class GoogleAuth
        // {
        //     [JsonPropertyName("access_token")]
        //     public string AccessToken { get; set; }
        //     [JsonPropertyName("refresh_token")]
        //     public string RefreshToken { get; set; }
        //     [JsonPropertyName("expire_in")]
        //     public int ExpiresIn { get; set; }
        //     public string Scope { get; set; }
        //     [JsonPropertyName("token_type")]
        //     public string TokenType { get; set; }
        //     [JsonPropertyName("id_token")]
        //     public string IdToken { get; set; }
        // }

        /// <summary>
        /// Authenticate to google with code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        /// <exception cref="UserException"></exception>
        // public async Task<(string, User)> GoogleAuthenticate(string code)
        // { 
        //     var res = await _client.PostAsync<GoogleAuth>($"https://oauth2.googleapis.com/token?code={code}&client_id={_googleId}&client_secret={_googleSecret}&redirect_uri=postmessage&grant_type=authorization_code");
        //     if (res == null)
        //         throw new ApiException("Failed to call API");

        //     JwtSecurityTokenHandler tokenHandler = new();
        //     string? id = tokenHandler.ReadJwtToken(res.IdToken).Payload["sub"].ToString();
        //     string? email = tokenHandler.ReadJwtToken(res.IdToken).Payload["email"].ToString();

        //     if (id == null)
        //         throw new ApiException("API did not return the necessary arguments");
        //     var user = _db.GetUserByGoogle(id);
        //     if (user == null)
        //     {
        //         if (email == null)
        //             throw new UserException("You must register before using OAuth");

        //         user = _db.GetUserByIdentifier(email);
        //         if (user == null)
        //             throw new UserException("You must register before using OAuth");
        //     }

        //     if (user.Google == null)
        //     {
        //         user.Google ??= new(id, res.RefreshToken);
        //         _db.SaveUser(user);
        //     }

        //     var tokenKey = Encoding.UTF8.GetBytes(_key);

        //     SecurityTokenDescriptor tokenDescriptor = new()
        //     {
        //         Subject = new(new Claim[]
        //         {
        //             new(ClaimTypes.Email, user.Email),
        //             new(ClaimTypes.Role, user.Role),
        //             new(ClaimTypes.Name, user.Id),
        //             new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
        //         }),

        //         Expires = DateTime.UtcNow.AddHours(1),

        //         SigningCredentials = new(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
        //     };

        //     var token = tokenHandler.CreateToken(tokenDescriptor);

        //     return (tokenHandler.WriteToken(token), user);
        // }

        /// <summary>
        /// Authenticate User and give him a token
        /// </summary>
        /// <param name="username">User username or email</param>
        /// <param name="password">User password</param>
        /// <param name="user">User informations</param>
        /// <returns>JWT</returns>
        public string Authenticate(string username, string password, out User user)
        {
            var found = _db.GetUserByIdentifier(username);

            if (found == null)
                throw new MongoException("User not found");
            if (!PasswordHash.VerifyPassword(password, found.Password))
                throw new UserException("Wrong password");

            user = found;
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
    }
}
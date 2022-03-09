using HookHook.Backend.Models;
using HookHook.Backend.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HookHook.Backend.Entities
{
    /// <summary>
    /// User Model
    /// </summary>
    [BsonIgnoreExtraElements]
    public class User
    {
        /// <summary>
        /// Database User-Object Id
        /// </summary>
        //[JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private init; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Random Secure ID to verify Email && forgotten password
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RandomId { get; set; }

        /// <summary>
        /// Have the user validated is email
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// User username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User first name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// User last name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [JsonIgnore]
        public string? Password { get; set; }

        /// <summary>
        /// User Role
        /// </summary>
        public string Role { get; set; } = "User";

        /// <summary>
        /// List of accounts the user can use to Logging in
        /// </summary>
        public Dictionary<Providers, OAuthAccount> OAuthAccounts { get; set; } = new();

        /// <summary>
        /// List of accoutns the user can use to create AREA
        /// </summary>
        public Dictionary<Providers, List<OAuthAccount>> ServicesAccounts { get; set; } = new();

        /// <summary>
        /// List of user's AREA
        /// </summary>
        public List<Area> Areas {get; set;} = new();

        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="email">Email</param>
        public User(string email) => 
            Email = email.ToLowerInvariant();

        /// <summary>
        /// User constructor from Controller form
        /// </summary>
        /// <param name="form">Controller form</param>
        public User(RegisterForm form) : this(form.Email)
        {
            Username = form.Username;
            FirstName = form.FirstName;
            LastName = form.LastName;
            Password = form.Password;
            GenerateRandomId();
        }

        /// <summary>
        /// Generate a new Random ID
        /// </summary>
        public void GenerateRandomId() =>
            RandomId = ObjectId.GenerateNewId().ToString();
    }

    /// <summary>
    /// Class containing OAuth information
    /// </summary>
    public class OAuthAccount
    {
        /// <summary>
        /// Service user ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Service Refresh Token
        /// </summary>
        public string? RefreshToken { get; set; }
        /// <summary>
        /// AccessToken expiration date
        /// </summary>
        public DateTime? ExpiresIn { get; set; }
        /// <summary>
        /// Service Secret Token
        /// </summary>
        public string? Secret { get; set; }
        /// <summary>
        /// Service Access Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="access">Access Token</param>
        /// <param name="date">Access Token lifespan</param>
        /// <param name="refresh">Refresh Token</param>
        /// <param name="secret">Secret Token</param>
        public OAuthAccount(string id, string access, TimeSpan? date = null, string? refresh = null, string? secret = null)
        {
            UserId = id;
            RefreshToken = refresh;
            if (date.HasValue)
                ExpiresIn = DateTime.UtcNow.Add(date.Value);
            AccessToken = access;
            Secret = secret;
        }
    }
}

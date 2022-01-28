using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace HookHook.Backend.Entities
{
    /// <summary>
    /// User Model
    /// </summary>
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

        public OAuthAccount? Google { get; set; }
        public OAuthAccount? Discord { get; set; }
        public OAuthAccount? Spotify { get; set; }
        public OAuthAccount? Twitter { get; set; }
        public OAuthAccount? Twitch { get; set; }
        public OAuthAccount? GitHub { get; set; }

        public List<Area> Areas {get; set;} = new();

        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="email">Email</param>
        public User(string email) => 
            Email = email;
    }

    public class OAuthAccount
    {
        public string UserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string AccessToken { get; set; }

        public OAuthAccount(string id, string access, TimeSpan? date = null, string? refresh = null)
        {
            UserId = id;
            RefreshToken = refresh;
            if (date.HasValue)
                ExpiresIn = DateTime.UtcNow.Add(date.Value);
            AccessToken = access;
        }
    }
}

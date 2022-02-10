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

        public OAuthAccount? GoogleOAuth { get; set; }
        public OAuthAccount? DiscordOAuth { get; set; }
        public OAuthAccount? SpotifyOAuth { get; set; }
        public TwitterAccount? TwitterOAuth { get; set; }
        public OAuthAccount? TwitchOAuth { get; set; }
        public OAuthAccount? GitHubOAuth { get; set; }

        public List<OAuthAccount> GoogleServices { get; set; } = new();
        public List<OAuthAccount> DiscordServices { get; set; } = new();
        public List<OAuthAccount> SpotifyServices { get; set; } = new();
        public List<TwitterAccount> TwitterServices { get; set; } = new();
        public List<OAuthAccount> TwitchServices { get; set; } = new();
        public List<OAuthAccount> GitHubServices { get; set; } = new();

        [BsonIgnore]
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

    public class TwitterAccount : OAuthAccount
    {
        public string OAuthSecret { get; set; }

        public TwitterAccount(string id, string access, string accessSecret, TimeSpan? date = null, string? refresh = null) : base(id, access, date, refresh)
        {
            OAuthSecret = accessSecret;
        }
    }
}

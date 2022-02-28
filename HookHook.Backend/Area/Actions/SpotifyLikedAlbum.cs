using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Spotify user liked an album action
    /// </summary>
    [Service(Providers.Spotify, "Liked a spotify album")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikedAlbum : IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "album.id", "album.name", "album.artists", "like.date"
        };

        /// <summary>
        /// Spotify service account ID
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List saved of album liked
        /// </summary>
        public List<string> StoredLibrary { get; private init; } = new();

        /// <summary>
        /// Client used to check on Spotify API
        /// </summary>
        private SpotifyClient? _spotifyClient;

        /// <summary>
        /// SpotifyLikedAlbum constructor
        /// </summary>
        /// <param name="accountId">Spotify service account ID</param>
        /// <param name="user">HookHook user</param>
        public SpotifyLikedAlbum(string accountId, User user)
        {
            AccountId = accountId;

            var albums = GetLikedAlbums(user).GetAwaiter().GetResult();

            foreach (var album in albums.Items!)
                StoredLibrary.Add(album.Album.Id);
        }

        /// <summary>
        /// Get all liked album list of user
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of album</returns>
        private async Task<Paging<SavedAlbum>> GetLikedAlbums(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var albums = await _spotifyClient.Library.GetAlbums();

            return albums;
        }

        /// <summary>
        /// Check if a new album is liked
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>A list of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var albums = await GetLikedAlbums(user);

            foreach (var item in albums.Items!)
            {
                if (StoredLibrary.Contains(item.Album.Id))
                    continue;
                StoredLibrary.Add(item.Album.Id);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], item.Album.Id },
                    { Formatters[1], item.Album.Name },
                    { Formatters[2], string.Join(", ", item.Album.Artists.Select(x => x.Name)) },
                    { Formatters[3], item.AddedAt.ToString("G") },
                };
                return (formatters, true);
            }

            return (null, false);
        }
    }
}
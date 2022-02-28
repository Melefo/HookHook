using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// Spotify liked music track action
    /// </summary>
    [Service(Providers.Spotify, "Liked a spotify music")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikedMusic : IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "track.id", "track.name", "track.artists", "like.date"
        };

        /// <summary>
        /// Spotify service account id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved music track
        /// </summary>
        public List<string> StoredLibrary { get; private init; } = new();

        /// <summary>
        /// Client used to check on Spotify API
        /// </summary>
        private SpotifyClient? _spotifyClient;

        /// <summary>
        /// SpotifyLikedMusic constructor
        /// </summary>
        /// <param name="accountId">Spotify service account Id</param>
        /// <param name="user">HookHook user</param>
        public SpotifyLikedMusic(string accountId, User user)
        {
            AccountId = accountId;

            var likedSongs = GetLikedSongs(user).GetAwaiter().GetResult();

            foreach (var song in likedSongs.Items!)
                StoredLibrary.Add(song.Track.Id);
        }

        /// <summary>
        /// Get list of all liked tracks
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of track</returns>
        private async Task<Paging<SavedTrack>> GetLikedSongs(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var tracks = await _spotifyClient.Library.GetTracks();

            return tracks;
        }

        /// <summary>
        /// Check if user liked a new music track
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of formatters</returns>
        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var tracks = await GetLikedSongs(user);

            foreach (var item in tracks.Items!)
            {
                if (StoredLibrary.Contains(item.Track.Id))
                    continue;
                StoredLibrary.Add(item.Track.Id);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], item.Track.Id },
                    { Formatters[1], item.Track.Name },
                    { Formatters[2], string.Join(", ", item.Track.Artists.Select(x => x.Name)) },
                    { Formatters[3], item.AddedAt.ToString("G") }
                };
                return (formatters, true);
            }

            return (null, false);
        }
    }
}
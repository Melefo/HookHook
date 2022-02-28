using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    /// <summary>
    /// Like a spotify album reaction
    /// </summary>
    [Service(Providers.Spotify, "Like a spotify album")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikeAlbum : IReaction
    {
        /// <summary>
        /// Album title
        /// </summary>
        public string AlbumTitle { get; set; }
        /// <summary>
        /// Artist name
        /// </summary>
        public string ArtistName { get; set; }
        /// <summary>
        /// Spotify service account Id
        /// </summary>
        public string AccountId { get; set; }


        /// <summary>
        /// CLient used to check on Spotify API
        /// </summary>
        private SpotifyClient? _spotifyClient;

        /// <summary>
        /// SpotifyLikeAlbum constructor
        /// </summary>
        /// <param name="albumTitle">Album title</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="accountId">Spotofy service account Id</param>
        public SpotifyLikeAlbum([ParameterName("Album title")] string albumTitle, [ParameterName("Artist name")] string artistName, string accountId)
        {
            AlbumTitle = albumTitle;
            ArtistName = artistName;
            AccountId = accountId;
        }

        /// <summary>
        /// Like the album
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatters from actions</param>
        /// <returns></returns>
        public async Task Execute(User user, Dictionary<string, object?> formatters)
        {
            var title = AlbumTitle.FormatParam(formatters);
            var artist = ArtistName.FormatParam(formatters);

            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            SearchRequest searchRequest = new (SearchRequest.Types.Album, $"{title} {artist}");
            var searchResults = await _spotifyClient.Search.Item(searchRequest);

            LibrarySaveAlbumsRequest saveAlbums = new(new List<string>()
            {
                searchResults.Albums.Items![0].Id
            });
            await _spotifyClient.Library.SaveAlbums(saveAlbums);
        }
    }
}
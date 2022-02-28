using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    /// <summary>
    /// Like a music on spotify reaction
    /// </summary>
    [Service(Providers.Spotify, "Like a spotify music")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikeMusic: IReaction
    {
        /// <summary>
        /// Song title
        /// </summary>
        public string SongTitle {get; set;}
        /// <summary>
        /// Artist name
        /// </summary>
        public string ArtistName {get; set;}
        /// <summary>
        /// Spotify service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Client used to check on Spotify API
        /// </summary>
        private SpotifyClient? _spotifyClient;

        /// <summary>
        /// SpotifyLikeMusic constructor
        /// </summary>
        /// <param name="songTitle">Song title</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="accountId">Spotify Service account Id</param>
        public SpotifyLikeMusic([ParameterName("Song title")] string songTitle, [ParameterName("Artist name")] string artistName, string accountId)
        {
            SongTitle = songTitle;
            ArtistName = artistName;
            AccountId = accountId;
        }

        /// <summary>
        /// Like the music
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatters from action</param>
        /// <returns></returns>
        public async Task Execute(User user, Dictionary<string, object?> formatters)
        {
            var title = SongTitle.FormatParam(formatters);
            var artist = ArtistName.FormatParam(formatters);

            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            SearchRequest searchRequest = new SearchRequest(SearchRequest.Types.Track, $"{title} {artist}");
            var searchResults = await _spotifyClient.Search.Item(searchRequest);

            LibrarySaveTracksRequest saveTracks = new (new List<string>() {searchResults.Tracks.Items![0].Id});
            await _spotifyClient.Library.SaveTracks(saveTracks);
        }
    }
}
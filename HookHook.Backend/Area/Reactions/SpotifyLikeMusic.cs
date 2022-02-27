using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    [Service(Providers.Spotify, "Like a spotify music")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikeMusic: IReaction
    {
        public string SongTitle {get; set;}
        public string ArtistName {get; set;}
        public string AccountId { get; set; }

        private SpotifyClient? _spotifyClient;

        public SpotifyLikeMusic([ParameterName("Song title")] string songTitle, [ParameterName("Artist name")] string artistName, string accountId)
        {
            SongTitle = songTitle;
            ArtistName = artistName;
            AccountId = accountId;
        }


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
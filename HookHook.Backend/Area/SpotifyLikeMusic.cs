using HookHook.Backend.Entities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    [BsonIgnoreExtraElements]
    public class SpotifyLikeMusic: IAction, IReaction
    {
        public string SongTitle {get; set;}
        public string ArtistName {get; set;}

        [BsonIgnore]
        private SpotifyClient _spotifyClient = null;

        public List<string> StoredLibrary { get; private init; } = new();

        public SpotifyLikeMusic(string songTitle, string artistName)
        {
            SongTitle = songTitle;
            ArtistName = artistName;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.SpotifyOAuth.AccessToken);

            var tracks = await _spotifyClient.Library.GetTracks();

            foreach (var item in tracks.Items) {
                if (StoredLibrary.Contains(item.Track.Id)) {
                    continue;
                }

                // todo save stored library

                StoredLibrary.Add(item.Track.Id);
                return (item.Track.Name, true);
            }

            return (null, false);
        }

        public async Task Execute(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.SpotifyOAuth.AccessToken);

            // * search song
            // * add song to library
            SearchRequest searchRequest = new SearchRequest(SearchRequest.Types.Track, $"{SongTitle} {ArtistName}");
            var searchResults = await _spotifyClient.Search.Item(searchRequest);

            LibrarySaveTracksRequest saveTracks = new (new List<string>() {searchResults.Tracks.Items[0].Id});
            await _spotifyClient.Library.SaveTracks(saveTracks);
        }
    }
}
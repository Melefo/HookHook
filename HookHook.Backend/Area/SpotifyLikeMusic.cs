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
    public class SpotifyLikeMusic: IAction, IReaction
    {
        public string SongTitle {get; set;}
        public string ArtistName {get; set;}

        [BsonIgnore]
        private SpotifyClient? _spotifyClient;

        public List<string> StoredLibrary { get; private init; } = new();

        public string AccountId { get; set; }

        public SpotifyLikeMusic(string songTitle, string artistName, string accountId, User userEntity)
        {
            SongTitle = songTitle;
            ArtistName = artistName;
            AccountId = accountId;

            var likedSongs = GetLikedSongs(userEntity).GetAwaiter().GetResult();

            foreach (var song in likedSongs.Items!) {
                StoredLibrary.Add(song.Track.Id);
            }
        }

        private async Task<Paging<SavedTrack>> GetLikedSongs(Entities.User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var tracks = await _spotifyClient.Library.GetTracks();

            return (tracks);
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var tracks = await GetLikedSongs(user);

            foreach (var item in tracks.Items!) {
                if (StoredLibrary.Contains(item.Track.Id)) {
                    continue;
                }

                StoredLibrary.Add(item.Track.Id);
                return (item.Track.Name, true);
            }

            return (null, false);
        }

        public async Task Execute(User user, string actionInfo)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            // * search song
            // * add song to library
            SearchRequest searchRequest = new SearchRequest(SearchRequest.Types.Track, $"{SongTitle} {ArtistName}");
            var searchResults = await _spotifyClient.Search.Item(searchRequest);

            // todo gestion d'erreur

            LibrarySaveTracksRequest saveTracks = new (new List<string>() {searchResults.Tracks.Items![0].Id});
            await _spotifyClient.Library.SaveTracks(saveTracks);
        }
    }
}
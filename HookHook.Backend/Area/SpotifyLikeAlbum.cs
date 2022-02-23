using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    [Service(Providers.Spotify, "Like a spotify album")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikeAlbum: IAction, IReaction
    {
        public string AlbumTitle {get; set;}
        public string ArtistName {get; set;}

        [BsonIgnore]
        private SpotifyClient? _spotifyClient;

        public List<string> StoredLibrary { get; private init; } = new();

        private string _serviceAccountId;

        public SpotifyLikeAlbum(string albumTitle, string artistName, string serviceAccountId)
        {
            AlbumTitle = albumTitle;
            ArtistName = artistName;
            _serviceAccountId = serviceAccountId;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == _serviceAccountId).AccessToken);

            var albums = await _spotifyClient.Library.GetAlbums();

            foreach (var item in albums.Items!) {

                DateTime dateAdded = item.AddedAt;
                // ! possible de trier avec dateAdded ;(

                if (StoredLibrary.Contains(item.Album.Id)) {
                    continue;
                }

                // todo save stored library

                StoredLibrary.Add(item.Album.Id);
                return (item.Album.Name, true);
            }

            return (null, false);
        }

        public async Task Execute(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.OAuthAccounts[Providers.Spotify].AccessToken);

            // * search album
            // * add album to library
            SearchRequest searchRequest = new SearchRequest(SearchRequest.Types.Album, $"{AlbumTitle} {ArtistName}");
            var searchResults = await _spotifyClient.Search.Item(searchRequest);

            // todo gestion d'erreur

            LibrarySaveAlbumsRequest saveAlbums = new (new List<string>() {searchResults.Albums.Items![0].Id});
            await _spotifyClient.Library.SaveAlbums(saveAlbums);
        }
    }
}
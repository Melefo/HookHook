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

        public string AccountId { get; set; }

        public SpotifyLikeAlbum([ParameterName( "Album title")] string albumTitle, [ParameterName("Artist name")] string artistName, string accountId, User userEntity)
        {
            AlbumTitle = albumTitle;
            ArtistName = artistName;
            AccountId = accountId;

            var albums = GetLikedAlbums(userEntity).GetAwaiter().GetResult();

            foreach (var album in albums.Items!) {
                StoredLibrary.Add(album.Album.Id);
            }
        }

        private async Task<Paging<SavedAlbum>> GetLikedAlbums(Entities.User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var albums = await _spotifyClient.Library.GetAlbums();

            return (albums);
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var albums = await GetLikedAlbums(user);

            foreach (var item in albums.Items!) {

                DateTime dateAdded = item.AddedAt;

                if (StoredLibrary.Contains(item.Album.Id)) {
                    continue;
                }

                StoredLibrary.Add(item.Album.Id);
                return (item.Album.Name, true);
            }

            return (null, false);
        }

        public async Task Execute(User user, string actionInfo)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

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
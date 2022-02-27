using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area
{
    [Service(Providers.Spotify, "Like a spotify album")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikeAlbum : IReaction
    {
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
        public string AccountId { get; set; }

        private SpotifyClient? _spotifyClient;

        public SpotifyLikeAlbum([ParameterName("Album title")] string albumTitle, [ParameterName("Artist name")] string artistName, string accountId)
        {
            AlbumTitle = albumTitle;
            ArtistName = artistName;
            AccountId = accountId;
        }

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
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using SpotifyAPI.Web;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Spotify, "Liked a spotify music")]
    [BsonIgnoreExtraElements]
    public class SpotifyLikedMusic: IAction
    {
        public string SongTitle {get; set;}
        public string AccountId { get; set; }
        public string ArtistName {get; set;}

        public string[] Formatters { get; } = new[]
        {
            "track.id", "track.name", "track.artists", "like.date"
        };
        public List<string> StoredLibrary { get; private init; } = new();

        private SpotifyClient? _spotifyClient;

        public SpotifyLikedMusic(string accountId, User userEntity)
        {
            AccountId = accountId;

            var likedSongs = GetLikedSongs(userEntity).GetAwaiter().GetResult();

            foreach (var song in likedSongs.Items!) {
                StoredLibrary.Add(song.Track.Id);
            }
        }

        private async Task<Paging<SavedTrack>> GetLikedSongs(User user)
        {
            _spotifyClient ??= new SpotifyClient(user.ServicesAccounts[Providers.Spotify].SingleOrDefault(acc => acc.UserId == AccountId)!.AccessToken);

            var tracks = await _spotifyClient.Library.GetTracks();

            return tracks;
        }

        public async Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var tracks = await GetLikedSongs(user);

            foreach (var item in tracks.Items!) {
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
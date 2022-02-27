using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
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

        public async Task<(string?, bool)> Check(User user)
        {
            var tracks = await GetLikedSongs(user);

            foreach (var item in tracks.Items!) {
                if (StoredLibrary.Contains(item.Track.Id))
                    continue;

                StoredLibrary.Add(item.Track.Id);
                return (item.Track.Name, true);
            }

            return (null, false);
        }
    }
}
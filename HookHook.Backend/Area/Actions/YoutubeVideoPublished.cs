using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [Service(Providers.Google, "Video is published")]
    [BsonIgnoreExtraElements]
    public class YoutubeVideoPublished: IAction
    {
        public string Channel {get; set;}

        [BsonIgnore]
        public GoogleService _googleService;

        public List<string> Videos { get; private init; } = new();

        private IConfiguration _config;

        private string _serviceAccountId;

        public YoutubeVideoPublished(string channel, GoogleService googleService, string serviceAccountId)
        {
            Channel = channel;
            _googleService = googleService;
            _serviceAccountId = serviceAccountId;
        }

        public Task<(string?, bool)> Check(User user)
        {
            var youtubeClient = _googleService.CreateYouTube(user.ServicesAccounts[Providers.Google].SingleOrDefault(acc => acc.UserId == _serviceAccountId));

            var channelsRequest = youtubeClient.Channels.List(Channel);
            channelsRequest.ForUsername = Channel;
            var channels = channelsRequest.Execute();
            var wantedChannel = channels.Items[0];

            var uploads = wantedChannel.ContentDetails.RelatedPlaylists.Uploads;

            // var videos = youtubeClient.Playlists.List(uploads);
            var videosRequest = youtubeClient.PlaylistItems.List(uploads);
            var videos = videosRequest.Execute();

            foreach (var video in videos.Items) {
                if (Videos.Contains(video.Id))
                    continue;

                // todo save
                Videos.Add(video.Id);
                return Task.FromResult<(string?, bool)>((video.Snippet.Title, true));
            }

            return Task.FromResult<(string?, bool)>((null, false));
        }


    }
}
using CoreTweet;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [Service("youtube", "Video is published")]
    [BsonIgnoreExtraElements]
    public class YoutubeVideoPublished: IAction
    {
        public string Channel {get; set;}

        [BsonIgnore]
        public YouTubeService _youtubeService;

        public List<string> Videos { get; private init; } = new();

        private IConfiguration _config;

        public YoutubeVideoPublished(string channel, YouTubeService youtubeService)
        {
            Channel = channel;
            _youtubeService = youtubeService;
        }

        public async Task<(string?, bool)> Check(User user)
        {
            var youtubeClient = _youtubeService.CreateYouTube(user);

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
                return (video.Snippet.Title, true);
            }

            return (null, false);
        }


    }
}
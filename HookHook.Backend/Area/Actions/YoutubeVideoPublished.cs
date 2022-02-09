using CoreTweet;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area
{
    [BsonIgnoreExtraElements]
    public class YoutubeVideoPublished: IAction
    {
        public string Channel {get; set;}

        [BsonIgnore]
        public YouTubeService _youtubeService;

        public List<long> Videos { get; private init; } = new();

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

            var videos = youtubeClient.Playlists.List(uploads);

            foreach (var tweet in tweets) {
                if (Tweets.Contains(tweet.Id))
                    continue;

                // todo save
                Tweets.Add(tweet.Id);
                return (tweet.FullText, true);
            }

            return (null, false);
        }


    }
}
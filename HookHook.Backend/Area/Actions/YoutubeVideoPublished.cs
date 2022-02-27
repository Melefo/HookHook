using Google.Apis.YouTube.v3.Data;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Actions
{
    [Service(Providers.Google, "Video is published")]
    [BsonIgnoreExtraElements]
    public class YoutubeVideoPublished: IAction
    {
        public string Channel {get; set;}
        public string AccountId { get; set; }

        public List<string> Videos { get; private init; } = new();

        private readonly GoogleService _googleService;

        public YoutubeVideoPublished(GoogleService service) =>
            _googleService = service;

        public YoutubeVideoPublished([ParameterName("Channel name")] string channel, string accountId, User user, GoogleService googleService) : this(googleService)
        {
            Channel = channel;
            AccountId = accountId;

            var videos = GetVideos(user);
            foreach (var video in videos)
                Videos.Add(video.Id);
        }

        public IList<PlaylistItem> GetVideos(User user)
        {
            var youtubeClient = _googleService.CreateYouTube(user.ServicesAccounts[Providers.Google].SingleOrDefault(acc => acc.UserId == AccountId)!);

            var searchChannel = youtubeClient.Search.List("snippet");
            searchChannel.Q = Channel;
            searchChannel.Type = "channel";
            var channels = searchChannel.Execute();
            var channelId = channels.Items[0].Id.ChannelId;

            var listChannels = youtubeClient.Channels.List("contentDetails");
            listChannels.Id = channelId;
            var channel = listChannels.Execute().Items[0];

            var uploads = channel.ContentDetails.RelatedPlaylists.Uploads;

            var videosRequest = youtubeClient.PlaylistItems.List("id, snippet");
            videosRequest.PlaylistId = uploads;
            videosRequest.MaxResults = 50;
            var videos = videosRequest.Execute();

            return videos.Items;
        }

        public Task<(string?, bool)> Check(User user)
        {
            var videos = GetVideos(user);

            foreach (var video in videos) {
                if (Videos.Contains(video.Id))
                    continue;

                Videos.Add(video.Id);
                return Task.FromResult<(string?, bool)>((video.Snippet.Title, true));
            }

            return Task.FromResult<(string?, bool)>((null, false));
        }


    }
}
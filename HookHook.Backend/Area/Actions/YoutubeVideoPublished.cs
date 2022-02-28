using Google.Apis.YouTube.v3.Data;
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using User = HookHook.Backend.Entities.User;

namespace HookHook.Backend.Area.Actions
{
    /// <summary>
    /// New YouTube video is published
    /// </summary>
    [Service(Providers.Google, "Video is published")]
    [BsonIgnoreExtraElements]
    public class YoutubeVideoPublished : IAction
    {
        /// <summary>
        /// List of formatters for reactions
        /// </summary>
        public static string[] Formatters { get; } = new[]
        {
            "video.title", "video.id", "video.description", "video.thumbnail"
        };

        /// <summary>
        /// YouTube channel name
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// YouTube service account Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// List of saved videos
        /// </summary>
        public List<string> Videos { get; private init; } = new();

        /// <summary>
        /// Service used to check on Google/YouTube API
        /// </summary>
        private readonly GoogleService _googleService;

        /// <summary>
        /// YouTubeVideoPublished constructor used by Mongo
        /// </summary>
        /// <param name="service">Google servive</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public YoutubeVideoPublished(GoogleService service) =>
            _googleService = service;

        /// <summary>
        /// YouTubeVideoPublished constructor
        /// </summary>
        /// <param name="channel">YouTube channel name</param>
        /// <param name="accountId">YouTube service account id</param>
        /// <param name="user">HookHook user</param>
        /// <param name="googleService">Google service</param>
        public YoutubeVideoPublished([ParameterName("Channel name")] string channel, string accountId, User user, GoogleService googleService) : this(googleService)
        {
            Channel = channel;
            AccountId = accountId;

            var videos = GetVideos(user);
            foreach (var video in videos)
                Videos.Add(video.Id);
        }

        /// <summary>
        /// Get all channel videos
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>list of videos</returns>
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

        /// <summary>
        /// Check if a new video is published
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <returns>List of formatters</returns>
        public Task<(Dictionary<string, object?>?, bool)> Check(User user)
        {
            var videos = GetVideos(user);

            foreach (var video in videos)
            {
                if (Videos.Contains(video.Id))
                    continue;
                Videos.Add(video.Id);

                var formatters = new Dictionary<string, object?>()
                {
                    { Formatters[0], video.Snippet.Title },
                    { Formatters[1], video.Id },
                    { Formatters[2], video.Snippet.Description },
                    { Formatters[3], video.Snippet.Thumbnails.High.Url }
                };
                return Task.FromResult<(Dictionary<string, object?>?, bool)>((formatters, true));
            }

            return Task.FromResult<(Dictionary<string, object?>?, bool)>((null, false));
        }


    }
}
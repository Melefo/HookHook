using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using Google.Apis.YouTube.v3.Data;

namespace HookHook.Backend.Area.Reactions
{
    /// <summary>
    /// Post a comment on a youtube video reaction
    /// </summary>
    [BsonIgnoreExtraElements]
    [Service(Providers.Google, "post a comment")]
    public class YoutubePostComment : IReaction
    {
        /// <summary>
        /// Video ID
        /// </summary>
        public string VideoId { get; private init; }
        /// <summary>
        /// Comment content
        /// </summary>
        public string Comment { get; private init; }
        /// <summary>
        /// YouTube service Account id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Service used to check on Google/YouTube API
        /// </summary>
        private readonly GoogleService _googleService;

        /// <summary>
        /// YoutubePostComment used by mondo
        /// </summary>
        /// <param name="service">Google service</param>
        /// <remarks>You should not use this constructor as not all members are initialized</remarks>
        public YoutubePostComment(GoogleService service) =>
            _googleService = service;

        /// <summary>
        /// YoutubePostComment constructor
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="comment">Comment content</param>
        /// <param name="googleService">Google service</param>
        /// <param name="accountId">Google service account ID</param>
        public YoutubePostComment([ParameterName("Video ID")] string videoId, [ParameterName("Comment content")] string comment, GoogleService googleService, string accountId) : this(googleService)
        {
            VideoId = videoId;
            Comment = comment;
            AccountId = accountId;
        }

        /// <summary>
        /// Post the comment
        /// </summary>
        /// <param name="user">HookHook user</param>
        /// <param name="formatters">Formatters from action</param>
        /// <returns></returns>
        public Task Execute(User user, Dictionary<string, object?> formatters)
        {
            var videoId = VideoId.FormatParam(formatters);
            var comment = Comment.FormatParam(formatters);

            var youtubeClient = _googleService.CreateYouTube(user.ServicesAccounts[Providers.Google].SingleOrDefault(acc => acc.UserId == AccountId)!);

            var commentThread = new CommentThread()
            {
                Snippet = new CommentThreadSnippet()
                {
                    VideoId = videoId,
                    TopLevelComment = new Comment()
                    {
                        Snippet = new()
                        {
                            TextOriginal = comment
                        }
                    }
                }
            };

            var insert = youtubeClient.CommentThreads.Insert(commentThread, "snippet");
            var search = insert.Execute();

            return Task.CompletedTask;
        }
    }
}
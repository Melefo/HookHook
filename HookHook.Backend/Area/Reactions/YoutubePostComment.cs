using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using Google.Apis.YouTube.v3.Data;

namespace HookHook.Backend.Area.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.Google, "post a comment")]
    public class YoutubePostComment : IReaction
    {
        public string VideoId { get; private init; }
        public string Comment { get; private init; }
        public string AccountId { get; set; }

        private readonly GoogleService _googleService;

        public YoutubePostComment(GoogleService service) =>
            _googleService = service;

        public YoutubePostComment([ParameterName("Video ID")] string videoId, [ParameterName("Comment content")] string comment, GoogleService googleService, string accountId) : this(googleService)
        {
            VideoId = videoId;
            Comment = comment;
            AccountId = accountId;
        }

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
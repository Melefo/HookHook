using HookHook.Backend.Utilities;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Reactions
{
    [BsonIgnoreExtraElements]
    [Service(Providers.Google, "post a comment")]
    public class YoutubePostComment : IReaction
    {
        public string VideoName {get; private init;}
        public string Comment {get; private init;}

        [BsonIgnore]
        private GoogleService _googleService;

        public string AccountId { get; set; }

        // * faudrait prendre le channelName aussi pour être sûr
        // * ou le lien plutôt... et à la limite on parse l'id dessus
        public YoutubePostComment(string videoName, string comment, GoogleService googleService, string serviceAccountId)
        {
            VideoName = videoName;
            Comment = comment;
            _googleService = googleService;
            AccountId = serviceAccountId;
        }

        public Task Execute(User user, string actionInfo)
        {
            var youtubeClient = _googleService.CreateYouTube(user.ServicesAccounts[Providers.Google].SingleOrDefault(acc => acc.UserId == AccountId)!);

            var searchRequest = youtubeClient.Search.List(VideoName);
            var search = searchRequest.Execute();

            var commentObject = new Google.Apis.YouTube.v3.Data.Comment();
            commentObject.Snippet.TextOriginal = Comment;
            // ! vraiment pas sûr que ceci fonctionne
            var commentRequest = youtubeClient.Comments.Update(commentObject, search.Items[0].Id.ToString());
            var commentResult = commentRequest.Execute();

            return Task.CompletedTask;
        }
    }
}
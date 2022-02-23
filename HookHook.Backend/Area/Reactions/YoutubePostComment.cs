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

        private string _serviceAccountId;

        // * faudrait prendre le channelName aussi pour être sûr
        // * ou le lien plutôt... et à la limite on parse l'id dessus
        public YoutubePostComment(string videoName, string comment, GoogleService googleService, string serviceAccountId)
        {
            VideoName = videoName;
            Comment = comment;
            _googleService = googleService;
            _serviceAccountId = serviceAccountId;
        }

        public async Task Execute(Entities.User user)
        {
            var youtubeClient = _googleService.CreateYouTube(user.ServicesAccounts[Providers.Google].SingleOrDefault(acc => acc.UserId == _serviceAccountId));

            var searchRequest = youtubeClient.Search.List(VideoName);
            var search = searchRequest.Execute();

            var commentObject = new Google.Apis.YouTube.v3.Data.Comment();
            commentObject.Snippet.TextOriginal = Comment;
            // ! vraiment pas sûr que ceci fonctionne
            var commentRequest = youtubeClient.Comments.Update(commentObject, search.Items[0].Id.ToString());
            var commentResult = commentRequest.Execute();
        }
    }
}
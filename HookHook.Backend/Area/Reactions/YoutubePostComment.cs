using HookHook.Backend.Utilities;
using HookHook.Backend.Models.Github;
using HookHook.Backend.Exceptions;
using HookHook.Backend.Entities;
using HookHook.Backend.Services;
using System.Net.Http.Headers;
using Octokit;
using HookHook.Backend.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace HookHook.Backend.Reactions
{
    [BsonIgnoreExtraElements]
    [Service("google", "post a comment")]
    public class YoutubePostComment : IReaction
    {
        public string VideoName {get; private init;}
        public string Comment {get; private init;}

        [BsonIgnore]
        private GoogleService _googleService;

        // * faudrait prendre le channelName aussi pour être sûr
        public YoutubePostComment(string videoName, string comment, GoogleService googleService)
        {
            VideoName = videoName;
            Comment = comment;
            _googleService = googleService;
        }

        public Task Execute(Entities.User user)
        {
            var youtubeClient = _googleService.CreateYouTube(user.OAuthAccounts[Providers.Google]);

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
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using static Google.Apis.YouTube.v3.YouTubeService;
using HookHook.Backend.Utilities;

namespace HookHook.Backend.Services
{
    public class GoogleService
    {
        private readonly string _googleId;
        private readonly string _googleSecret;
        private readonly string _googleKey;

        /// <summary>
        /// Youtube Service Constructor
        /// </summary>
        /// <param name="config"></param>
        public GoogleService(IConfiguration config)
        {
            _googleId = config["Google:ClientId"];
            _googleSecret = config["Google:ClientSecret"];
            _googleKey = config["Google:ApiKey"];
        }

        /// <summary>
        /// Create a Youtube Widget from a Google account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public YouTubeService CreateYouTube(User user)
        {
            if (!user.OAuthAccounts.TryGetValue(Providers.Google, out var oauth)) {
                throw (new ApiException("Not authenticated with google"));
            }
            return new(new BaseClientService.Initializer()
            {
                ApiKey = _googleKey,
                HttpClientInitializer = new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _googleId,
                        ClientSecret = _googleSecret
                    },
                    // ! jsp quel scope permet de commenter quelque chose
                    Scopes = new[] { Scope.YoutubeReadonly, Scope.Youtube, Scope.Youtubepartner, Scope.YoutubeUpload, Scope.YoutubeForceSsl }
                }), oauth.UserId, new TokenResponse()
                {
                    RefreshToken = oauth.RefreshToken,
                })
            });
        }
    }
}
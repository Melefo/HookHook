using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using HookHook.Backend.Exceptions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using static Google.Apis.YouTube.v3.YouTubeService;

namespace HookHook.Backend.Services
{

    public class YouTubeService
    {
        private readonly string _googleId;
        private readonly string _googleSecret;
        private readonly string _googleKey;

        /// <summary>
        /// Youtube Service Constructor
        /// </summary>
        /// <param name="config"></param>
        public YouTubeService(IConfiguration config)
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
        public Google.Apis.YouTube.v3.YouTubeService CreateYouTube(User user)
        {
            if (user.GoogleOAuth == null) {
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
                    Scopes = new[] { Scope.YoutubeReadonly, Scope.Youtube, Scope.Youtubepartner, Scope.YoutubeUpload }
                }), user.GoogleOAuth.UserId, new TokenResponse()
                {
                    RefreshToken = user.GoogleOAuth.RefreshToken,
                })
            });
        }
    }
}
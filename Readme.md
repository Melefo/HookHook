# HookHook

## List services

| Services | Events |
|----------|---------|
| Spotify | <ul> <li>SpotifyLikedAlbum (action/reaction)</li> Parameters: <ul> <li>Album title</li> <li>Artist name</li> </ul> Formatters: <ul> <li>album.id</li> <li>album.name</li> <li>album.artists</li> <li>like.date</li> </ul> <li>SpotifyLikedMusic (action/reaction)</li> Parameters: <ul> <li>Song title</li> <li>Artist name</li> </ul> Formatters: <ul> <li>track.id</li> <li>track.name</li> <li>track.artists</li> <li>like.date</li> </ul></ul>
| Twitter | <ul><li>TwitterFollowUser (action/reaction)</li>Parameters:<ul><li>Username (When user follows an account)</li></ul>Formatters:<ul><li>following.id</li><li>following.name</li><li>following.username</li></ul><li>Tweet (reaction)</li>Parameters:<ul><li>Tweet content</li></ul>Formatters:<ul><li>track.id</li><li>track.name</li><li>track.artists</li><li>like.date</li></ul><li>TweetHashtag (action)</li>Parameters:<ul><li>Hashtag</li></ul>Formatters: <ul> <li>tweet.text</li> <li>tweet.date</li> <li>tweet.id</li> <li>tweet.source</li> </ul>
| Discord | <ul> <li>DiscordWebhook (reaction)</li> Parameters <ul> <li>Webhook URL</li> <li>Message title</li> <li>Message content</li> </ul> <li>DiscordPinned (action)</li> Parameters <ul> <li>Guild ID</li> <li>Channel ID</li> </ul> Formatters <ul> <li>msg.content</li> <li>msg.id</li> <li>author.id</li> <li>author.name</li> <li>author.mention</li> <li>msg.date</li> </ul></ul>
| GitHub | <ul> <li>GithubCreateIssue (reaction)</li> Parameters <ul> <li>Username</li> <li>Repository</li> <li>Issue title</li> <li>Issue body</li> </ul> <li>GithubCreateRepository (reaction)</li> Parameters <ul> <li>Repository name</li> <li>Repository description</li> </ul> <li>GithubIssueCreated (action)</li> Parameters <ul> <li>Username</li> <li>Repository name</li> </ul> Formatters <ul> <li>issue.title</li> <li>issue.id</li> <li>issue.body</li> <li>issue.date</li> <li>issue.url</li> <li>author.id</li> <li>author.name</li> <li>author.login</li> </ul> <li>GithubNewCommit (action)</li> Parameters <ul> <li>Username</li> <li>Repository name</li> </ul> Formatters <ul><li>commit.sha</li> <li>commit.msg</li> <li>author.id</li> <li>author.name</li> <li>author.name</li> <li>author.login</li> </ul> <li>GithubNewRepository (action)</li> Parameters <ul> <li>Username</li> </ul> Formatters <ul> <li>repo.name</li> <li>repo.id</li> <li>repo.date</li> <li>repo.description</li> <li>repo.url</li> </ul></ul>
| Google | <ul> <li>YoutubePostComment (reaction)</li> Parameters <ul> <li>Video ID</li> <li>Comment content</li> </ul> <li>YoutubeVideoPublished (action)</li> Parameters <ul> <li>Channel name</li> </ul> Formatters <ul> <li>video.title</li> <li>video.id</li> <li>video.description</li> <li>video.thumbnail</li> </ul></ul>
| Twitch | <ul> <li>TwitchLiveStarted (action)</li> Parameters <ul> <li>Username</li> </ul> Formatters <ul> <li>stream.game</li> <li>stream.id</li> <li>stream.date</li> <li>stream.thumbnail</li> <li>stream.title</li> </ul></ul>

---
## Admins

If you are an administrator user you will have access to the admin area:
![adminArea](./docs/examples/admin_btn.png)

On the admin area page, you can:
<ul>
    <li>
        See all registered users
    </li>
    <li>
        Delete a user
    </li>
    <li>
        Modify a user's role (Admin/User)
    </li>
</ul>

![adminAreaPage](./docs/examples/admin_table.png)

---

## Developer guide

### <u>Environment variables</u>

At the root of the project folder, create a <i>.env</i> file.
This file will be read by docker to create environment variables and must contain values for the following keys:

### The Mongo database:
<ul>
    <li>
        MONGO_USERNAME = Mongo root user
    </li>
    <li>
        MONGO_PASSWORD = Root password
    </li>
    <li>
        MONGO_DATABASE = Name of Database used by the backend
    </li>
</ul>

### JWT
<ul>
    <li>
        JWTKEY = A random string with at least 16 chars
    </li>
</ul>

### SMTP

<ul>
    <li>
        SMTP_EMAIL = Email address of your SMTP server
    </li>
    <li>
        SMTP_PASSWORD = Password of your SMTP server
    </li>
</ul>

### The services:


([Create a Google App](https://console.cloud.google.com/apis/credentials)):
<ul>
    <li>
        GOOGLE_CLIENTID = ClientId of your Google app
    </li>
    <li>
        GOOGLE_CLIENTSECRET = Client secret of your Google app
    </li>
    <li>
        GOOGLE_APIKEY = API key of your Google app
    </li>
    <li>
        GOOGLE_REDIRECT = Redirect URL for Google OAuth
    </li>
</ul>

([Create a Discord App](https://discord.com/developers/docs/intro)):
<ul>
    <li>
        DISCORD_CLIENTID = ClientId of your discord app
    </li>
    <li>
        DISCORD_CLIENTSECRET = Client secret of your discord app
    </li>
    <li>
        DISCORD_REDIRECT = Redirect URL for Discord OAuth
    </li>
    <li>
        DISCORD_BOTTOKEN = Bot token
    </li>
</ul>

([Create a Github App](https://docs.github.com/en/developers)):
<ul>
    <li>
        GITHUB_CLIENTID = ClientId of your Github app
    </li>
    <li>
        GITHUB_CLIENTSECRET = Client secret of your Github app
    </li>
</ul>

([Create a Spotify App](https://developer.spotify.com/)):
<ul>
    <li>
        SPOTIFY_CLIENTID = ClientId of your Spotify app
    </li>
    <li>
        SPOTIFY_CLIENTSECRET = Client secret of your Spotify app
    </li>
    <li>
        SPOTIFY_REDIRECT = Redirect URL for Spotify OAuth
    </li>
</ul>

([Create a Twitch App](https://dev.twitch.tv/)):
<ul>
    <li>
        TWITCH_CLIENTID = ClientId of your Twitch app
    </li>
    <li>
        TWITCH_CLIENTSECRET = Client secret of your Twitch app
    </li>
    <li>
        TWITCH_REDIRECT = Redirect URL for Twitch OAuth
    </li>
</ul>

([Create a Twitter App](https://developer.twitter.com/en)):
<ul>
    <li>
        TWITTER_CLIENTID = ClientId of your Twitter app
    </li>
    <li>
        TWITTER_CLIENTSECRET = Client secret of your Twitter app
    </li>
    <li>
        TWITTER_REDIRECT = Redirect URL for Twitter OAuth
    </li>
</ul>

Once you have all your environment variables set you can perform the following command:
```
docker-compose up --build
```

Frontend will be available at: http://localhost:80/

Backend will be available at: http://localhost:8080/

Mongo will be available at: mongodb://MONGO_USERNAME:MONGO_PASSWORD@localhost:27017/

## API limits

Be warned of the following API limitations:
### <u>Google</u>:

10'000 requests per day

### <u>Twitter</u>:

You may not tweet the same message twice.
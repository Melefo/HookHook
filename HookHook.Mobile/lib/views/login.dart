import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter_twitch_auth/flutter_twitch_auth.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:http/http.dart';
import 'package:pkce/pkce.dart';
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/views/forgot_password.dart';
import 'package:hookhook/views/home.dart';
import 'package:hookhook/views/register.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:uni_links/uni_links.dart';
import 'package:url_launcher/url_launcher.dart';
import '../main.dart';
import 'package:validators/validators.dart' as validators;

class LoginView extends StatefulWidget {
  const LoginView({Key? key}) : super(key: key);

  static String routeName = "/login";

  @override
  _LoginView createState() => _LoginView();
}

class _LoginView extends AdaptiveState<LoginView> {
  TextEditingController username = TextEditingController();
  TextEditingController password = TextEditingController();
  late StreamSubscription googleListener;
  late StreamSubscription spotifyListener;
  late StreamSubscription githubListener;
  late StreamSubscription twitterListener;

  String? error;
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  Future<void> validateAndSave() async {
    final FormState form = _formKey.currentState!;
    if (!form.validate()) {
      return;
    }
    try {
      await HookHook.backend.signIn.login(
          username.value.text, password.value.text);
    }
    on TimeoutException {
      setState(() {
        error = "Failed to call HookHook instance";
        password.clear();
      });
    }
    on ClientException catch (e) {
      setState(() {
        error = e.message;
        password.clear();
      });
    }
    setState(() {
      error = null;
    });
    if (await HookHook.backend.signIn.token != null) {
      await Navigator.pushReplacementNamed(
          context, HomeView.routeName);
    }
  }

  @override
  void initState() {
    super.initState();
    googleListener = google.onCurrentUserChanged.listen((
        GoogleSignInAccount? account) async {
      await HookHook.backend.signIn.google(account!.serverAuthCode!);
      if (await HookHook.backend.signIn.token != null) {
        await Navigator.pushReplacementNamed(
            context, HomeView.routeName
        );
      }
    });
    spotifyListener = linkStream.listen((String? response) async {
      if (response!.startsWith("hookhook://oauth/spotify")) {
        final url = Uri.parse(response);
        await HookHook.backend.signIn.spotify(url.queryParameters["code"]!);
        if (await HookHook.backend.signIn.token != null) {
          await Navigator.pushReplacementNamed(
              context, HomeView.routeName
          );
        }
      }
    });
    githubListener = linkStream.listen((String? response) async {
      if (response!.startsWith("hookhook://oauth/github")) {
        final url = Uri.parse(response);
        await HookHook.backend.signIn.github(url.queryParameters["code"]!);
        if (await HookHook.backend.signIn.token != null) {
          await Navigator.pushReplacementNamed(
              context, HomeView.routeName
          );
        }
      }
    });
    twitterListener = linkStream.listen((String? response) async {
      if (response!.startsWith("hookhook://oauth/twitter")) {
        final url = Uri.parse(response);
        String code = url.queryParameters["oauth_token"]!;
        String verifier = url.queryParameters["oauth_verifier"]!;
        await HookHook.backend.signIn.twitter(code, verifier);
        if (await HookHook.backend.signIn.token != null) {
          await Navigator.pushReplacementNamed(
              context, HomeView.routeName
          );
        }
      }
    });
  }

  @override
  void dispose() {
    googleListener.cancel();
    spotifyListener.cancel();
    githubListener.cancel();
    twitterListener.cancel();
    super.dispose();
  }

  GoogleSignIn google = GoogleSignIn(
    // Optional clientId
    clientId: const String.fromEnvironment('GOOGLE_CLIENTID'),
    scopes: [
      "openid",
      "email",
      "profile",
      "https://www.googleapis.com/auth/youtube",
      "https://www.googleapis.com/auth/youtube.readonly",
      "https://www.googleapis.com/auth/youtube.force-ssl"
    ],
  );

  Future<void> redirect(String authUri) async {
    if (await canLaunch(authUri)) {
      await launch(authUri);
    }
  }

  Widget constructGoogle() =>
      IconButton(
          onPressed: () async {
            try {
              await google.signIn();
            } catch (error) {
              if (kDebugMode) {
                print(error);
              }
            }
          },
          icon: ServicesIcons.google(100),
          iconSize: 0.08.sw
      );

  Widget constructGitHub() =>
      IconButton(
        onPressed: () async {
          final scopes = [
            "user",
            "repo"
          ];
          await redirect(
              "https://github.com/login/oauth/authorize?client_id=${const String.fromEnvironment('GITHUB_CLIENTID')}&redirect_uri=hookhook://oauth/github&response_type=code&scope=${scopes.join(' ')}");
        },
        icon: ServicesIcons.gitHub(100),
        iconSize: 0.08.sw,
      );

  Widget constructSpotify() =>
      IconButton(
        onPressed: () async {
          final scopes = [
            "user-read-email",
            "user-read-private",
            "user-library-modify",
            "user-library-read",
            "playlist-modify-private",
            "playlist-read-private",
            "playlist-modify-public",
          ];
          await redirect(
              "https://accounts.spotify.com/authorize?client_id=${const String.fromEnvironment('SPOTIFY_CLIENTID')}&redirect_uri=hookhook://oauth/spotify&response_type=code&scope=${scopes.join(" ")}");
        },
        icon: ServicesIcons.spotify(100),
        iconSize: 0.08.sw,
      );

  Widget constructDiscord() {
    final pkcePair = PkcePair.generate();

    return IconButton(
      onPressed: () async {
        final listener = linkStream.listen(null);
        listener.onData((String? response) async {
          if (response!.startsWith("hookhook://oauth/discord")) {
            final url = Uri.parse(response);
            await HookHook.backend.signIn.discord(
                url.queryParameters["code"]!, pkcePair.codeVerifier);
          }
          if (await HookHook.backend.signIn.token != null) {
            await Navigator.pushReplacementNamed(
                context, HomeView.routeName);
          }
          listener.cancel();
        });

        final scopes = [
          "identify",
          "guilds",
          "email",
          "bot"
        ];
        await redirect(
            "https://discord.com/oauth2/authorize?code_challenge=${pkcePair.codeChallenge}&code_challenge_method=S256&client_id=${const String.fromEnvironment('DISCORD_CLIENTID')}&redirect_uri=hookhook://oauth/discord&response_type=code&scope=${scopes.join(' ')}&permissions=66568");
      },
      icon: ServicesIcons.discord(100),
      iconSize: 0.08.sw,
    );
  }

  Widget constructTwitch() =>
      IconButton(
        onPressed: () async {
          final scopes = [
            "channel:read:subscriptions",
            "channel:manage:broadcast",
            "user:read:broadcast",
            "user:read:subscriptions",
            "user:edit",
            "user:read:email",
            "user:read:follows"
          ];
          FlutterTwitchAuth.initialize(
              twitchClientId: const String.fromEnvironment('TWITCH_CLIENTID'),
              twitchRedirectUri: const String.fromEnvironment('TWITCH_REDIRECT'),
              twitchClientSecret: '',
              scope: scopes.join(' ')
          );
          String? code = await FlutterTwitchAuth.authToCode(context);
          await HookHook.backend.signIn.twitch(code!);
          if (await HookHook.backend.signIn.token != null) {
            await Navigator.pushReplacementNamed(
                context, HomeView.routeName
            );
          }
        },
        icon: ServicesIcons.twitch(100),
        iconSize: 0.08.sw,
      );

  Widget constructTwitter() =>
      IconButton(
        onPressed: () async {
          String? url = await HookHook.backend.signIn.authorize(
              "Twitter", "hookhook://oauth/twitter");
          await redirect(url!);
        },
        icon: ServicesIcons.twitter(100),
        iconSize: 0.08.sw,
      );

  List<Widget> generateFromServices() {
    List<Widget> list = [];
    if (HookHook.backend.about == null) {
      return [];
    }

    for (var service in HookHook.backend.about!.server.services) {
      switch (service.name.toLowerCase()) {
        case "spotify":
          {
            list.add(constructSpotify());
            break;
          }
        case "discord":
          {
            list.add(constructDiscord());
            break;
          }
        case "github":
          {
            list.add(constructGitHub());
            break;
          }
        case "google":
        case "youtube":
          {
            //list.add(constructGoogle());
            break;
          }
        case "twitch":
          {
            list.add(constructTwitch());
            break;
          }
        case "twitter":
          {
            list.add(constructTwitter());
            break;
          }
      }
    }
    return list;
  }

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors
              .light,
          body: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  Image.asset(
                      "assets/pinguin/warp.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  ),
                  const WelcomeHookHook(),
                  Image.asset(
                      "assets/pinguin/warp.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  )
                ],
              ),
              Text(
                  "Try to login!",
                  style: TextStyle(
                      fontSize: 14.sp,
                      color: darkMode ? Colors.white : Colors.black
                  )
              ),
              if (error != null)
                Padding(
                  padding: const EdgeInsets.only(top: 16),
                  child: Text(
                      error!,
                  style: const TextStyle(
                    color: Colors.redAccent
                  )),
                ),
              Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 0.15.sw
                ),
                child: Form(
                  key: _formKey,
                  child: Column(
                    children: [
                      TextFormField(
                        decoration: InputDecoration(
                          enabledBorder: UnderlineInputBorder(
                              borderSide: BorderSide(
                                  color: darkMode ? Colors.white : Colors.black
                              )
                          ),
                          labelText: "Username/Email",
                          labelStyle: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        ),
                        style: TextStyle(
                            color: darkMode ? Colors.white : Colors.black
                        ),
                        controller: username,
                        validator: (text) {
                          if (text == null || text.length < 2) {
                            return "Username must be at least 2 characters";
                          }
                          if (text.length > 256) {
                            return "Username cannot exceed 256 characters";
                          }
                          return null;
                        },
                      ),
                      TextFormField(
                          decoration: InputDecoration(
                            enabledBorder: UnderlineInputBorder(
                                borderSide: BorderSide(
                                    color: darkMode ? Colors.white : Colors.black
                                )
                            ),
                            labelText: "Password",
                            labelStyle: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                          controller: password,
                          obscureText: true,
                        validator: (text) {
                          if (text == null || text.length < 4) {
                            return "Password must be at least 4 characters";
                          }
                          if (text.length > 256) {
                            return "Password cannot exceed 256 characters";
                          }
                          return null;
                        },
                      ),
                      TextButton(
                        onPressed: () async =>
                        await Navigator.pushNamed(
                            context, ForgotPassword.routeName),
                        child: Text(
                          "Forgot password?",
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black,
                              decoration: TextDecoration.underline
                          ),
                        ),
                      ),
                      TextFormField(
                        decoration: InputDecoration(
                          enabledBorder: UnderlineInputBorder(
                              borderSide: BorderSide(
                                  color: darkMode ? Colors.white : Colors.black
                              )
                          ),
                          labelText: "HookHook Instance",
                          labelStyle: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        ),
                        style: TextStyle(
                            color: darkMode ? Colors.white : Colors.black
                        ),
                        initialValue: Backend.apiEndpoint,
                        validator: (text) {
                          if (!validators.isURL(text, requireProtocol: true)) {
                            return "A valid URL must be entered";
                          }
                          return null;
                        },
                        onChanged: (text) async {
                          if (text[text.length - 1] != '/') {
                            text += '/';
                          }
                          String? token = await HookHook.storage.read(
                              key: Backend.tokenKey);
                          String? username = await HookHook.storage.read(
                              key: Backend.usernameKey);
                          String? password = await HookHook.storage.read(
                              key: Backend.passwordKey);
                          await Backend.init(text, token, username, password);
                          await HookHook.storage.write(
                              key: Backend.instanceKey,
                              value: Backend.apiEndpoint
                          );
                          setState(() {

                          });
                        },
                      ),
                      const Padding(
                        padding: EdgeInsets.all(8),
                      ),
                      TextButton(
                          onPressed: () async => await validateAndSave(),
                          child: Text(
                            "Login",
                            style: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          style: TextButton.styleFrom(
                              backgroundColor: darkMode
                                  ? HookHookColors.gray
                                  : Colors.white,
                              padding: const EdgeInsets.all(15),
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10)
                              ),
                              minimumSize: const Size(150, 0)
                          )
                      ),
                      const Padding(
                        padding: EdgeInsets.all(4),
                      ),
                      TextButton(
                          onPressed: () async =>
                          await Navigator.pushNamed(
                              context, RegisterView.routeName),
                          child: Text(
                            "Register",
                            style: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          style: TextButton.styleFrom(
                              backgroundColor: darkMode
                                  ? HookHookColors.gray
                                  : Colors.white,
                              padding: const EdgeInsets.all(15),
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10)
                              ),
                              minimumSize: const Size(150, 0)
                          )
                      ),
                    ],
                  ),
                ),
              ),
              const Padding(
                padding: EdgeInsets.all(8),
              ),
              Row(
                children: generateFromServices(),
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              ),
            ],
          )
      );
}
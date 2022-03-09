import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_twitch_auth/flutter_twitch_auth.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:pkce/pkce.dart';
import 'package:uni_links/uni_links.dart';
import 'package:url_launcher/url_launcher.dart';
import '../adaptive_state.dart';
import '../hookhook_colors.dart';
import '../main.dart';
import '../services_icons.dart';
import 'package:validators/validators.dart' as validators;

import 'home.dart';

class RegisterView extends StatefulWidget {
  const RegisterView({Key? key}) : super(key: key);

  static String routeName = "/register";

  @override
  _RegisterView createState() => _RegisterView();
}

class _RegisterView extends AdaptiveState<RegisterView> {
  TextEditingController firstName = TextEditingController();
  TextEditingController lastName = TextEditingController();
  TextEditingController email = TextEditingController();
  TextEditingController username = TextEditingController();
  TextEditingController password = TextEditingController();
  TextEditingController confirm = TextEditingController();

  late StreamSubscription googleListener;
  late StreamSubscription spotifyListener;
  late StreamSubscription githubListener;
  late StreamSubscription twitterListener;

  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  bool _sent = false;

  String? error;

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

  Future<void> validateAndSave() async {
    final FormState form = _formKey.currentState!;
    if (!form.validate()) {
      return;
    }
    try {
      final errors = await HookHook.backend.signIn.register(
          firstName.value.text, lastName.value.text, email.value.text,
          username.value.text, password.value.text
      );
      if (errors != null) {
        setState(() {
          if (errors.password != null) {
            error = errors.password;
          }
          if (errors.username != null) {
            error = errors.username;
          }
          if (errors.email != null) {
            error = errors.email;
          }
        });
        return;
      }
    }
    on TimeoutException {
      setState(() {
        error = "Failed to call HookHook instance";
        confirm.clear();
      });
      return;
    }
    setState(() {
      error = null;
      _sent = true;
    });
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
        backgroundColor: darkMode ? HookHookColors.dark : HookHookColors.light,
        body: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                Image.asset(
                    "assets/pinguin/breakdance.gif",
                    height: 0.15.sw,
                    width: 0.15.sw
                ),
                const WelcomeHookHook(),
                Image.asset(
                    "assets/pinguin/breakdance.gif",
                    height: 0.15.sw,
                    width: 0.15.sw
                )
              ],
            ),
            Text(
                "Wanna try our service?",
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
            if (_sent)
              Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 0.05.sw,
                    vertical: 16
                ),
                child: Column(
                  children: [
                    Text(
                        "You are successfully registered!",
                        style: TextStyle(
                            fontSize: 12.sp,
                        color: darkMode ? Colors.white : Colors.black
                        ),
                        textAlign: TextAlign.center
                    ),
                    Text(
                        "Please check your email inbox for verification link",
                        style: TextStyle(
                            fontSize: 12.sp,
                        color: darkMode ? Colors.white : Colors.black
                        ),
                        textAlign: TextAlign.center
                    )
                  ],
                ),
              ),
            Padding(
              padding: EdgeInsets.symmetric(
                  horizontal: 0.15.sw
              ),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                              decoration: InputDecoration(
                                  enabledBorder: UnderlineInputBorder(
                                      borderSide: BorderSide(
                                          color: darkMode ? Colors.white : Colors.black
                                      )
                                  ),
                                  labelText: "First name",
                                labelStyle: TextStyle(
                                    color: darkMode ? Colors.white : Colors.black
                                ),
                              ),
                              style: TextStyle(
                                  color: darkMode ? Colors.white : Colors.black
                              ),
                              controller: firstName,
                              validator: (text) {
                                if (text == null || text.isEmpty) {
                                  return "Your first name is required";
                                }
                                return null;
                              },
                            ),
                          ),
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                              decoration: InputDecoration(
                                enabledBorder: UnderlineInputBorder(
                                    borderSide: BorderSide(
                                        color: darkMode ? Colors.white : Colors.black
                                    )
                                ),
                                  labelText: "Last name",
                                labelStyle: TextStyle(
                                    color: darkMode ? Colors.white : Colors.black
                                ),
                              ),
                              style: TextStyle(
                                  color: darkMode ? Colors.white : Colors.black
                              ),
                              controller: lastName,
                                validator: (text) {
                                  if (text == null || text.isEmpty) {
                                    return "Your last name is required";
                                  }
                                  return null;
                                }
                            ),
                          )
                        ]
                    ),
                    TextFormField(
                      decoration: InputDecoration(
                          enabledBorder: UnderlineInputBorder(
                              borderSide: BorderSide(
                                  color: darkMode ? Colors.white : Colors.black
                              )
                          ),
                          labelText: "Email",
                        labelStyle: TextStyle(
                            color: darkMode ? Colors.white : Colors.black
                        ),
                      ),
                      style: TextStyle(
                          color: darkMode ? Colors.white : Colors.black
                      ),
                      controller: email,
                        validator: (text) {
                          if (text == null || !validators.isEmail(text)) {
                            return "You must enter a valid email";
                          }
                          return null;
                        }
                    ),
                    TextFormField(
                      decoration: InputDecoration(
                          enabledBorder: UnderlineInputBorder(
                              borderSide: BorderSide(
                                  color: darkMode ? Colors.white : Colors.black
                              )
                          ),
                          labelText: "Username",
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
                          return "Your username must contains at least 2 characters";
                        }
                        if (text.length > 256) {
                          return "Your username cannot exceed 256 characters";
                        }
                        return null;
                      }
                    ),
                    Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
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
                                    return "At least 4 characters";
                                  }
                                  if (text.length > 256) {
                                    return "Cannot exceed 256 characters";
                                  }
                                  return null;
                              },
                            ),
                          ),
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                              decoration: InputDecoration(
                                  enabledBorder: UnderlineInputBorder(
                                      borderSide: BorderSide(
                                          color: darkMode ? Colors.white : Colors.black
                                      )
                                  ),
                                  labelText: "Confirm password",
                                labelStyle: TextStyle(
                                    color: darkMode ? Colors.white : Colors.black
                                ),
                              ),
                              style: TextStyle(
                                  color: darkMode ? Colors.white : Colors.black
                              ),
                              controller: confirm,
                              obscureText: true,
                              validator: (text) {
                                if (text != password.value.text) {
                                  return "Doesn't match password";
                                }
                                return null;
                              },
                            ),
                          ),
                        ]
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
                        padding: EdgeInsets.all(8)
                    ),
                    TextButton(
                        onPressed: () async => await validateAndSave(),
                        child: Text(
                          "Register",
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        ),
                        style: TextButton.styleFrom(
                            backgroundColor: darkMode ? HookHookColors.gray : Colors.white,
                            padding: const EdgeInsets.all(15),
                            shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(10)
                            ),
                            minimumSize: const Size(150, 0)
                        )
                    )
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
        ),
      );
}
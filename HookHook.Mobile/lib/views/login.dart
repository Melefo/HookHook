import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
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
import 'package:flutter/src/widgets/image.dart' as ImageWidget;
import '../main.dart';

class LoginView extends StatefulWidget {
  const LoginView({Key? key}) : super(key: key);

  static String routeName = "/login";

  @override
  _LoginView createState() => _LoginView();
}

class _LoginView extends AdaptiveState<LoginView> {
  TextEditingController username = TextEditingController();
  TextEditingController password = TextEditingController();

  @override
  void initState() {
    super.initState();
    linkStream.listen(onListen);
  }

  void onListen(String? response) async {
    if (response!.startsWith(dotenv.env["SPOTIFY_REDIRECT"]!)) {
      final url = Uri.parse(response);
      await HookHook.backend.signIn.spotify(url.queryParameters["code"]!);
    }
    if (HookHook.backend.signIn.token != null) {
      await Navigator.pushReplacementNamed(
          context, HomeView.routeName);
    }
  }

  Future<void> redirect(String authUri) async {
    if (await canLaunch(authUri)) {
      await launch(authUri);
    }
  }

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
          await redirect("https://accounts.spotify.com/authorize?client_id=${dotenv.env['SPOTIFY_CLIENTID']}&redirect_uri=${dotenv.env['SPOTIFY_REDIRECT']}&response_type=code&scope=${scopes.join(" ")}");
        },
        icon: ServicesIcons.custom("spotify", 100),
        iconSize: 0.08.sw,
      );

  List<Widget> generateFromServices() {
    List<Widget> list = [];
    if (HookHook.backend.about == null) {
      return [];
    }

    for (var service in HookHook.backend.about!.server.services) {
      switch (service.name.toLowerCase()) {
        case "spotify": {
          list.add(constructSpotify());
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
                  ImageWidget.Image.asset(
                      "assets/pinguin/warp.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  ),
                  WelcomeHookHook(),
                  ImageWidget.Image.asset(
                      "assets/pinguin/warp.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  )
                ],
              ),
              Text(
                  "Try to login!",
                  style: TextStyle(
                      fontSize: 14.sp
                  )
              ),
              Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 0.15.sw
                ),
                child: Column(
                  children: [
                    TextFormField(
                      decoration: const InputDecoration(
                          border: UnderlineInputBorder(),
                          labelText: "Username/Email"
                      ),
                      controller: username,
                    ),
                    TextFormField(
                        decoration: const InputDecoration(
                            border: UnderlineInputBorder(),
                            labelText: "Password"
                        ),
                        controller: password,
                        obscureText: true
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
                      decoration: const InputDecoration(
                        border: UnderlineInputBorder(),
                        labelText: "HookHook Instance",
                      ),
                      initialValue: Backend.apiEndpoint,
                      onChanged: (text) async {
                        if (text[text.length - 1] != '/') {
                          text += '/';
                        }
                        await Backend.init(instance: text);
                        await HookHook.storage.write(
                            key: Backend.instanceKey, value: Backend.apiEndpoint
                        );
                        setState(() {

                        });
                      },
                    ),
                    const Padding(
                      padding: EdgeInsets.all(8),
                    ),
                    TextButton(
                        onPressed: () async {
                          await HookHook.backend.signIn.login(
                              username.value.text, password.value.text);
                          if (HookHook.backend.signIn.token != null) {
                            await Navigator.pushReplacementNamed(
                                context, HomeView.routeName);
                          }
                        },
                        child: Text(
                          "Login",
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        ),
                        style: ButtonStyle(
                            backgroundColor: MaterialStateProperty.all(
                                darkMode ? HookHookColors.gray : Colors.white
                            ),
                            padding: MaterialStateProperty.all(
                                const EdgeInsets.all(15)),
                            shape: MaterialStateProperty.all(
                                RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(10)
                                )
                            ),
                            minimumSize: MaterialStateProperty.all(
                                const Size(150, 0)
                            )
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
                        style: ButtonStyle(
                            backgroundColor: MaterialStateProperty.all(
                                darkMode ? HookHookColors.gray : Colors.white
                            ),
                            padding: MaterialStateProperty.all(
                                const EdgeInsets.all(15)),
                            shape: MaterialStateProperty.all(
                                RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(10)
                                )
                            ),
                            minimumSize: MaterialStateProperty.all(
                                const Size(150, 0)
                            )
                        )
                    ),
                  ],
                ),
              ),
              const Padding(
                padding: EdgeInsets.all(8),
              ),
              Row(
                children: generateFromServices(),
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              ),
              /*TextButton(
                onPressed: () {
                  setDarkMode(!darkMode);
                },
                child: Text(
                  darkMode ? 'Passer au light mode' : "Passer au dark mode",
                  style: TextStyle(
                      color: darkMode ? Colors.black : Colors.white
                  )
                ),
                style: ButtonStyle(
                    backgroundColor: MaterialStateProperty.all(
                        darkMode ? HookHookColors.blue : HookHookColors
                            .orange)
                ),
              )*/
            ],
          )
      );
}
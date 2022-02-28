import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';
import 'package:hookhook/wrapper/backend.dart';

class LoginView extends StatefulWidget {
  const LoginView({Key? key}) : super(key: key);

  static String routeName = "/login";

  @override
  _LoginView createState() => _LoginView();
}

class _LoginView extends AdaptiveState<LoginView> {
  List<Widget> generateFromServices() {
    List<Widget> list = [];
    for (var service in Backend().about.server.services) {
      list.add(ServicesIcons.custom(service.name.toLowerCase(), 50));
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
                  Image.asset("assets/pinguin/warp.gif", height: 100, width: 100),
                  WelcomeHookHook(),
                  Image.asset("assets/pinguin/warp.gif", height: 100, width: 100)
                ],
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 92),
                child: Column(
                  children: [
                    TextFormField(
                        decoration: InputDecoration(
                            border: UnderlineInputBorder(),
                            labelText: "Username/Email"
                        )
                    ),
                    TextFormField(
                        decoration: InputDecoration(
                            border: UnderlineInputBorder(),
                            labelText: "Password"
                        )
                    ),
                    TextButton(
                      onPressed: () => {},
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
                        border: UnderlineInputBorder(),
                        labelText: "HookHook Instance",
                      ),
                      initialValue: Backend.apiEndpoint,
                      onChanged: (text) async {
                        Backend.apiEndpoint = text;
                        await Backend.init();
                      },
                    ),
                    const Padding(
                      padding: EdgeInsets.all(8),
                    ),
                    TextButton(
                        onPressed: () => {},
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
                          minimumSize: MaterialStateProperty.all(Size(150, 0))
                        )
                    ),
                    const Padding(
                      padding: EdgeInsets.all(4),
                    ),
                    TextButton(
                        onPressed: () => {},
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
                            minimumSize: MaterialStateProperty.all(Size(150, 0))
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
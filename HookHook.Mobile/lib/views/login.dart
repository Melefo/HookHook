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

  List<Widget> generateFromServices() {
    List<Widget> list = [];
    if (HookHook.backend.about == null) {
      return [];
    }
    for (var service in HookHook.backend.about!.server.services) {
      list.add(ServicesIcons.custom(service.name.toLowerCase(), 0.08.sw));
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
                  WelcomeHookHook(),
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
import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/widgets/hookhook_title.dart';
import 'package:jwt_decode/jwt_decode.dart';

import '../hookhook_colors.dart';
import 'login.dart';

class SettingsView extends StatefulWidget {
  const SettingsView({Key? key}) : super(key: key);

  static String routeName = "/settings";

  @override
  _Settings createState() => _Settings();
}

class _Settings extends AdaptiveState<SettingsView> {
  String? token;

  @override
  void initState() {
    HookHook.backend.signIn.token.then((value) {
      setState(() {
        token = value;
      });
    });
    super.initState();
  }

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors
              .light,
          body: Column(
            children: [
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: HookHookTitle(),
              ),
              Row(
                  children: <Widget>[
                    Padding(
                        padding: const EdgeInsets.all(16.0),
                        child: Text(
                          "Enable app night mode",
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        )
                    ),
                    Expanded(
                        child: Divider(color: darkMode
                            ? Colors.white.withAlpha(50)
                            : Colors.black.withAlpha(50))
                    ),
                  ]
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Padding(
                    padding: const EdgeInsets.only(left: 60),
                    child: Text(
                        "Night Mode",
                      style: TextStyle(
                          color: darkMode ? Colors.white : Colors.black
                      )
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(right: 50),
                    child: Switch(
                      value: darkMode,
                      activeColor: HookHookColors.blue,
                      inactiveThumbColor: HookHookColors.orange,
                      onChanged: (bool value) {
                        setDarkMode(value);
                      },
                    ),
                  ),
                ],
              ),
              Row(
                  children: <Widget>[
                    Padding(
                        padding: const EdgeInsets.all(16.0),
                        child: Text(
                          "Current account",
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                        )
                    ),
                    Expanded(
                        child: Divider(color: darkMode
                            ? Colors.white.withAlpha(50)
                            : Colors.black.withAlpha(50))
                    ),
                  ]
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Padding(
                    padding: const EdgeInsets.only(left: 60),
                    child: Text(
                        token != null ? Jwt.parseJwt(token!)["given_name"] ?? "No name" : "",
                        style: TextStyle(
                            color: darkMode ? Colors.white : Colors.black
                        )
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(right: 50),
                    child: TextButton(
                        onPressed: () async {
                          await HookHook.backend.signIn.logout();
                          Navigator.pushReplacementNamed(context, LoginView.routeName);
                        },
                        child: Text(
                          "Logout",
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
                    ),
                  ),
                ],
              ),
            ],
          )
      );
}
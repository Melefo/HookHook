import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/widgets/hookhook_title.dart';

import '../hookhook_colors.dart';

class SettingsView extends StatefulWidget {
  const SettingsView({Key? key}) : super(key: key);

  static String routeName = "/settings";

  @override
  _Settings createState() => _Settings();
}

class _Settings extends AdaptiveState<SettingsView> {
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
                          "Enabled night mode in preferences",
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
                      activeColor: Colors.pink,
                      inactiveThumbColor: Colors.blue,
                      onChanged: (bool value) {
                        setDarkMode(value);
                      },
                    ),
                  ),
                ],
              ),
            ],
          )
      );
}
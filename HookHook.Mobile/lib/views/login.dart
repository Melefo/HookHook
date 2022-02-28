import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/services_icons.dart';
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
          body: Padding(
            padding: const EdgeInsets.symmetric(
                horizontal: 16
            ),
            child: Column(
              children: [
                const Padding(
                    padding: EdgeInsets.only(
                        top: 79
                    )
                ),
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
                  child: Text("Forgot password?"),
                  style: ButtonStyle(
                    backgroundColor: MaterialStateProperty.all(Colors.white),
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
                TextButton(
                    onPressed: () => {},
                    child: Text("Login")
                ),
                TextButton(
                    onPressed: () => {},
                    child: Text("Register"),
                    style: ButtonStyle(
                    ),
                ),
                Row(
                  children: generateFromServices(),
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                ),
                TextButton(
                  onPressed: () {
                    setDarkMode(!darkMode);
                  },
                  child: Text(darkMode ? 'Passer au light mode' : "Passer au dark mode", style: TextStyle(color: darkMode ? Colors.black : Colors.white),),
                  style: ButtonStyle(
                    backgroundColor: MaterialStateProperty.all(darkMode ? HookHookColors.blue : HookHookColors.orange)
                  ),
                )
              ],
            ),
          )
      );
}
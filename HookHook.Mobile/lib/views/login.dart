import 'package:flutter/material.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/wrapper/backend.dart';

class LoginView extends StatelessWidget {

  static String routeName = "/login";

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
                    child: Text("Register")
                ),
                Row(
                  children: generateFromServices(),
                )
              ],
            ),
          )
      );
}
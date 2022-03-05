import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';
import '../adaptive_state.dart';
import '../hookhook_colors.dart';
import '../main.dart';
import '../services_icons.dart';

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

  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  bool _sent = false;

  Future<void> validateAndSave() async {
    final FormState form = _formKey.currentState!;
    if (!form.validate()) {
      return;
    }
    await HookHook.backend.signIn.register(
        firstName.value.text, lastName.value.text, email.value.text,
        username.value.text, password.value.text
    );
    setState(() {
      _sent = true;
    });
  }

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
                    fontSize: 14.sp
                )
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
                            fontSize: 12.sp
                        ),
                        textAlign: TextAlign.center
                    ),
                    Text(
                        "Please check your email inbox for verification link",
                        style: TextStyle(
                            fontSize: 12.sp
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
                              decoration: const InputDecoration(
                                  border: UnderlineInputBorder(),
                                  labelText: "First name"
                              ),
                              controller: firstName,
                            ),
                          ),
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                              decoration: const InputDecoration(
                                  border: UnderlineInputBorder(),
                                  labelText: "Last name"
                              ),
                              controller: lastName,
                            ),
                          )
                        ]
                    ),
                    TextFormField(
                      decoration: const InputDecoration(
                          border: UnderlineInputBorder(),
                          labelText: "Email"
                      ),
                      controller: email,
                    ),
                    TextFormField(
                      decoration: const InputDecoration(
                          border: UnderlineInputBorder(),
                          labelText: "Username"
                      ),
                      controller: username,
                    ),
                    Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                                decoration: const InputDecoration(
                                    border: UnderlineInputBorder(),
                                    labelText: "Password"
                                ),
                                controller: password,
                                obscureText: true
                            ),
                          ),
                          ConstrainedBox(
                            constraints: BoxConstraints.tightFor(
                                width: 0.32.sw),
                            child: TextFormField(
                              decoration: const InputDecoration(
                                  border: UnderlineInputBorder(),
                                  labelText: "Confirm password"
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
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';

import '../main.dart';

class ForgotPassword extends StatefulWidget {
  const ForgotPassword({Key? key}) : super(key: key);

  static String routeName = "/forgot";

  @override
  _ForgotPassword createState() => _ForgotPassword();
}

class _ForgotPassword extends AdaptiveState<ForgotPassword> {
  TextEditingController username = TextEditingController();
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  bool _visible = false;

  Future<void> validateAndSave() async {
    final FormState form = _formKey.currentState!;
    if (!form.validate()) {
      return;
    }
    await HookHook.backend.signIn.forgotPassword(username.value.text);
    username.clear();
    setState(() {
      _visible = true;
    });
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
                    "assets/pinguin/clap.gif",
                    height: 0.15.sw,
                    width: 0.15.sw
                ),
                const WelcomeHookHook(),
                Image.asset(
                    "assets/pinguin/clap.gif",
                    height: 0.15.sw,
                    width: 0.15.sw
                )
              ],
            ),
            Text(
                "Forgot your password?",
                style: TextStyle(
                    fontSize: 14.sp,
                    color: darkMode ? Colors.white : Colors.black
                )
            ),
            if (_visible)
              Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 0.05.sw,
                    vertical: 16
                ),
                child: Text(
                    "If an account with this email or username exists an email has been sent to recover your password",
                    style: TextStyle(
                        fontSize: 12.sp,
                        color: darkMode ? Colors.white : Colors.black
                    ),
                    textAlign: TextAlign.center
                ),
              ),
            Padding(
              padding: EdgeInsets.symmetric(
                  horizontal: 0.15.sw,
                  vertical: 16
              ),
              child: Form(
                key: _formKey,
                child: TextFormField(
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
                  controller: username,
                  style: TextStyle(
                      color: darkMode ? Colors.white : Colors.black
                  ),
                  validator: (text) {
                    if (text == null || text.length < 2) {
                      return "Username must be at least 2 characters";
                    }
                    return null;
                  },
                ),
              ),
            ),
            TextButton(
                onPressed: () async => await validateAndSave(),
                child: Text(
                  "Send",
                  style: TextStyle(
                      color: darkMode ? Colors.white : Colors.black
                  ),
                ),
                style: TextButton.styleFrom(
                    backgroundColor: darkMode ? HookHookColors.gray : Colors
                        .white,
                    padding: const EdgeInsets.all(15),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)
                    ),
                    minimumSize: const Size(150, 0)
                )
            )
          ],
        ),
      );
}
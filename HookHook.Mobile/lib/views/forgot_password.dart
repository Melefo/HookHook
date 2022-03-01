import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';

class ForgotPassword extends StatefulWidget {
  const ForgotPassword({Key? key}) : super(key: key);

  static String routeName = "/forgot";

  @override
  _ForgotPassword createState() => _ForgotPassword();
}

class _ForgotPassword extends AdaptiveState<ForgotPassword> {
  TextEditingController username = TextEditingController();

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
                Image.asset("assets/pinguin/clap.gif", height: 0.15.sw,
                    width: 0.15.sw),
                WelcomeHookHook(),
                Image.asset("assets/pinguin/clap.gif", height: 0.15.sw,
                    width: 0.15.sw)
              ],
            ),
            Text(
                "Forgot your password?",
                style: TextStyle(
                  fontSize: 14.sp
                )
            ),
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 0.15.sw, vertical: 32),
              child: TextFormField(
                decoration: const InputDecoration(
                    border: UnderlineInputBorder(),
                    labelText: "Username/Email"
                ),
                controller: username,
              ),
            ),

            TextButton(
                onPressed: () => {},
                child: Text(
                  "Send",
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
      );
}
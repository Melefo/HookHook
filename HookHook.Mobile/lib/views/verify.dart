import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';

class Verify extends StatefulWidget {
  final String id;

  const Verify({Key? key, required this.id}) : super(key: key);

  static String routeName = "/verify/";

  @override
  _Verify createState() => _Verify();
}

class _Verify extends AdaptiveState<Verify> {
  Future<void> verify() async {
    await HookHook.backend.signIn.verifyEmail(widget.id);
    await SystemNavigator.pop(animated: true);
  }

  @override
  Widget build(BuildContext context) {
    verify();
    return Scaffold(
      body: Center(
        child: Column(
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
            const Padding(
                padding: EdgeInsets.all(32)
            ),
            Text(
                "Please wait...",
                style: TextStyle(
                    color: darkMode ? Colors.white : Colors.black
                )
            ),
            Text(
                "Your email is being validated",
                style: TextStyle(
                    color: darkMode ? Colors.white : Colors.black
                )
            ),
            Text(
                "The deeplink will close when finished",
                style: TextStyle(
                    color: darkMode ? Colors.white : Colors.black
                )
            )
          ],
        ),
      ),
    );
  }
}
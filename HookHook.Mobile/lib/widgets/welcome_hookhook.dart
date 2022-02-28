import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/hookhook_colors.dart';

import '../adaptive_state.dart';

class WelcomeHookHook extends StatefulWidget {
  @override
  _WelcomeHookHook createState() => _WelcomeHookHook();

}

class _WelcomeHookHook extends AdaptiveState<WelcomeHookHook> with SingleTickerProviderStateMixin {
  late Animation<Color?> animation;
  late AnimationController controller;

  @override
  void initState() {
    super.initState();
    controller =
        AnimationController(duration: const Duration(seconds: 2), vsync: this);
    animation =
    ColorTween(begin: HookHookColors.orange, end: HookHookColors.blue).animate(
        controller)
      ..addListener(() {
        setState(() {});
      });
    controller.forward();
    controller.repeat(reverse: true);
  }

  @override
  Widget build(BuildContext context) =>
      Padding(
        padding: const EdgeInsets.only(bottom: 16),
        child: RichText(
          textScaleFactor: 1.7,
          text: TextSpan(
            style: DefaultTextStyle.of(context).style,
            children: <TextSpan>[
              TextSpan(
                  text: "Welcome to H",
                  style: TextStyle(
                      color: darkMode ? Colors.white : Colors.black
                  )
              ),
              TextSpan(
                  text: "oo",
                  style: TextStyle(
                      color: animation.value
                  )
              ),
              TextSpan(
                  text: "kH",
                  style: TextStyle(
                      color: darkMode ? Colors.white : Colors.black
                  )
              ),
              TextSpan(
                  text: "oo",
                  style: TextStyle(
                      color: animation.value
                  )
              ),
              TextSpan(
                  text: "k!",
                  style: TextStyle(
                      color: darkMode ? Colors.white : Colors.black
                  )
              )
            ],
          ),
        ),
      );
}
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_svg/svg.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/views/login.dart';
import 'package:hookhook/views/settings.dart';
import 'package:jwt_decode/jwt_decode.dart';

import '../adaptive_state.dart';
import '../main.dart';

class HookHookTitle extends StatefulWidget {
  const HookHookTitle({Key? key, this.welcome = false}) : super(key: key);

  final bool welcome;

  @override
  _HookHookTitle createState() => _HookHookTitle();
}

class _HookHookTitle extends AdaptiveState<HookHookTitle> {
  String? token;

  @override
  void initState() {
    HookHook.backend.signIn.token.then((value) {
      setState(() {
        token = value;
      });
      if (value == null) {
        Navigator.pushReplacementNamed(context, LoginView.routeName);
      }
    });
    super.initState();
  }

  @override
  Widget build(BuildContext context) =>
      Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              RichText(
                  text: TextSpan(
                      style: TextStyle(
                          fontSize: 42.sp,
                          fontFamily: "Comfortaa"
                      ),
                      children: [
                        TextSpan(
                            text: "H",
                            style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black,
                            )
                        ),
                        WidgetSpan(
                            child: Stack(
                              clipBehavior: Clip.none,
                              alignment: Alignment.bottomCenter,
                              children: <Widget>[
                                Text(
                                    "oo",
                                    style: TextStyle(
                                        color: HookHookColors.orange,
                                        fontSize: 42.sp,
                                        fontFamily: "Comfortaa"
                                    )
                                ),
                                Positioned(
                                  bottom: -20,
                                  child: SvgPicture.asset(
                                    "assets/img/orangeBeak.svg",
                                    width: 0.07.sw,
                                  ),
                                )
                              ],
                            )
                        ),
                        TextSpan(
                            text: "kH",
                            style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black,
                            )
                        ),
                        WidgetSpan(
                            child: Stack(
                              clipBehavior: Clip.none,
                              alignment: Alignment.bottomCenter,
                              children: <Widget>[
                                Text(
                                    "oo",
                                    style: TextStyle(
                                        color: HookHookColors.blue,
                                        fontSize: 42.sp,
                                        fontFamily: "Comfortaa"
                                    )
                                ),
                                Positioned(
                                  bottom: -20,
                                  child: SvgPicture.asset(
                                    "assets/img/blueBeak.svg",
                                    width: 0.07.sw,
                                  ),
                                )
                              ],
                            )
                        ),
                        TextSpan(
                            text: "k",
                            style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black,
                            )
                        ),
                      ]
                  )
              )
            ],
          ),
          const Padding(
              padding: EdgeInsets.only(
                top: 16,
              )
          ),
          if (widget.welcome)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    RichText(
                        text: TextSpan(
                            style: TextStyle(
                                fontSize: 10.sp,
                                fontFamily: "Comfortaa"
                            ),
                            children: [
                              TextSpan(
                                  text: "Hello",
                                  style: TextStyle(
                                    color: darkMode ? Colors.white : Colors
                                        .black,
                                  )
                              ),
                              if (token != null)
                                TextSpan(
                                    text: " ${Jwt.parseJwt(
                                        token!)["given_name"]}",
                                    style: TextStyle(
                                      color: darkMode ? HookHookColors.blue : HookHookColors.orange,
                                    )
                                ),
                              TextSpan(
                                  text: ", welcome back! ",
                                  style: TextStyle(
                                    color: darkMode ? Colors.white : Colors
                                        .black,
                                  )
                              ),
                            ]
                        )
                    ),
                    TextButton(
                      onPressed: () =>
                          Navigator.pushNamed(context, SettingsView.routeName),
                      style: TextButton.styleFrom(
                          backgroundColor: darkMode
                              ? HookHookColors.gray
                              : Colors.white,
                          padding: const EdgeInsets.all(12),
                          minimumSize: Size.zero,
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10)
                          )
                      ),
                      clipBehavior: Clip.none,
                      child: Icon(
                          Icons.settings,
                          color: darkMode
                              ? Colors.white
                              : HookHookColors.gray
                      ),
                    )
                  ]
              ),
            )
        ],
      );
}
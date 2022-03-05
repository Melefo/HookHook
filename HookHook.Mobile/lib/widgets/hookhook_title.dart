import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_svg/svg.dart';
import 'package:hookhook/hookhook_colors.dart';

import '../adaptive_state.dart';

class HookHookTitle extends StatefulWidget {
  const HookHookTitle({Key? key}) : super(key: key);

  @override
  _HookHookTitle createState() => _HookHookTitle();
}

class _HookHookTitle extends AdaptiveState {
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
                          fontSize: 48.sp,
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
                                        fontSize: 48.sp,
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
                                        fontSize: 48.sp,
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
          Row()
        ],
      );
}
import 'dart:ui';

import 'package:flutter_svg/flutter_svg.dart';

class ServicesIcons {
  static SvgPicture custom(String name, double size, [Color? color]) {
    return (
      SvgPicture.asset("assets/img/${name.toLowerCase()}.svg",
          width: size,
          height: size,
        color: color,
      )
    );
  }

  static SvgPicture gitHub(double size, [Color? color]) =>
      SvgPicture.asset(color == null ? "assets/img/github.svg" : "assets/img/github2.svg",
        width: size,
        height: size,
        color: color,
      );

  static SvgPicture discord(double size, [Color? color]) =>
      SvgPicture.asset("assets/img/discord.svg",
        width: size,
        height: size,
        color: color,
      );

  static SvgPicture google(double size, [Color? color]) =>
      SvgPicture.asset("assets/img/google.svg",
        width: size,
        height: size,
        color: color,
      );

  static SvgPicture twitter(double size, [Color? color]) =>
      SvgPicture.asset("assets/img/twitter.svg",
        width: size,
        height: size,
        color: color,
      );

  static SvgPicture twitch(double size, [Color? color]) =>
      SvgPicture.asset(color == null ? "assets/img/twitch.svg" : "assets/img/twitch2.svg",
        width: size,
        height: size,
        color: color,
      );

  static SvgPicture spotify(double size, [Color? color]) =>
      SvgPicture.asset("assets/img/spotify.svg",
        width: size,
        height: size,
        color: color,
      );
}
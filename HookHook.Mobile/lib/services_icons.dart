import 'package:flutter_svg/flutter_svg.dart';

class ServicesIcons {
  static SvgPicture custom(String name, double size) {
    return (
      SvgPicture.asset("assets/img/${name.toLowerCase()}.svg",
          width: size,
          height: size
      )
    );
  }

  static SvgPicture gitHub(double size) =>
      SvgPicture.asset("assets/img/github.svg",
        width: size,
        height: size,
      );

  static SvgPicture discord(double size) =>
      SvgPicture.asset("assets/img/discord.svg",
        width: size,
        height: size,
      );

  static SvgPicture google(double size) =>
      SvgPicture.asset("assets/img/google.svg",
        width: size,
        height: size,
      );

  static SvgPicture twitter(double size) =>
      SvgPicture.asset("assets/img/twitter.svg",
        width: size,
        height: size,
      );

  static SvgPicture twitch(double size) =>
      SvgPicture.asset("assets/img/twitch.svg",
        width: size,
        height: size,
      );

  static SvgPicture spotify(double size) =>
      SvgPicture.asset("assets/img/spotify.svg",
        width: size,
        height: size,
      );
}
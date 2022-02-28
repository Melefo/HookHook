import 'package:flutter/material.dart';

class Colors {
  static final Map<int, Color> _orangeLuminance = {
    50: const Color.fromRGBO(240, 144, 19, .1),
    100: const Color.fromRGBO(240, 144, 19, .2),
    200: const Color.fromRGBO(240, 144, 19, .3),
    300: const Color.fromRGBO(240, 144, 19, .4),
    400: const Color.fromRGBO(240, 144, 19, .5),
    500: const Color.fromRGBO(240, 144, 19, .6),
    600: const Color.fromRGBO(240, 144, 19, .7),
    700: const Color.fromRGBO(240, 144, 19, .8),
    800: const Color.fromRGBO(240, 144, 19, .9),
    900: const Color.fromRGBO(240, 144, 19, 1),
  };

  static MaterialColor orange = MaterialColor(0xFFF09013, _orangeLuminance);

  static final Map<int, Color> _blueLuminance = {
    50: const Color.fromRGBO(163, 231, 238, .1),
    100: const Color.fromRGBO(163, 231, 238, .2),
    200: const Color.fromRGBO(163, 231, 238, .3),
    300: const Color.fromRGBO(163, 231, 238, .4),
    400: const Color.fromRGBO(163, 231, 238, .5),
    500: const Color.fromRGBO(163, 231, 238, .6),
    600: const Color.fromRGBO(163, 231, 238, .7),
    700: const Color.fromRGBO(163, 231, 238, .8),
    800: const Color.fromRGBO(163, 231, 238, .9),
    900: const Color.fromRGBO(163, 231, 238, 1),
  };

  static MaterialColor blue = MaterialColor(0xFFA3E7EE, _blueLuminance);

  static final Map<int, Color> _lightLuminance = {
    50: const Color.fromRGBO(240, 240, 240, .1),
    100: const Color.fromRGBO(240, 240, 240, .2),
    200: const Color.fromRGBO(240, 240, 240, .3),
    300: const Color.fromRGBO(240, 240, 240, .4),
    400: const Color.fromRGBO(240, 240, 240, .5),
    500: const Color.fromRGBO(240, 240, 240, .6),
    600: const Color.fromRGBO(240, 240, 240, .7),
    700: const Color.fromRGBO(240, 240, 240, .8),
    800: const Color.fromRGBO(240, 240, 240, .9),
    900: const Color.fromRGBO(240, 240, 240, 1),
  };

  static MaterialColor light = MaterialColor(0xFFF0F0F0, _lightLuminance);

  static final Map<int, Color> _darkLuminance = {
    50: const Color.fromRGBO(24, 26, 30, .1),
    100: const Color.fromRGBO(24, 26, 30, .2),
    200: const Color.fromRGBO(24, 26, 30, .3),
    300: const Color.fromRGBO(24, 26, 30, .4),
    400: const Color.fromRGBO(24, 26, 30, .5),
    500: const Color.fromRGBO(24, 26, 30, .6),
    600: const Color.fromRGBO(24, 26, 30, .7),
    700: const Color.fromRGBO(24, 26, 30, .8),
    800: const Color.fromRGBO(24, 26, 30, .9),
    900: const Color.fromRGBO(24, 26, 30, 1),
  };

  static MaterialColor dark = MaterialColor(0xFF181A1E, _darkLuminance);
}
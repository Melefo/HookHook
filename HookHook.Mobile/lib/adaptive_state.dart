import 'package:adaptive_theme/adaptive_theme.dart';
import 'package:flutter/material.dart';

abstract class AdaptiveState<T extends StatefulWidget> extends State {
  bool darkMode = false;
  dynamic _savedThemeMode;

  @override
  void initState() {
    super.initState();
    _getCurrentTheme();
    AdaptiveTheme
        .of(context)
        .modeChangeNotifier
        .addListener(() {
      setState(() {
        darkMode = !darkMode;
      });
    });
  }

  void _getCurrentTheme() {
    _savedThemeMode = AdaptiveTheme.of(context).theme;
    if (_savedThemeMode.toString() == 'AdaptiveThemeMode.dark') {
      setState(() {
        darkMode = true;
      });
    } else {
      setState(() {
        darkMode = false;
      });
    }
  }

  void setDarkMode(bool value) {
    if (value == true) {
      AdaptiveTheme.of(context).setDark();
    } else {
      AdaptiveTheme.of(context).setLight();
    }
    setState(() {
      darkMode = value;
    });
  }
}
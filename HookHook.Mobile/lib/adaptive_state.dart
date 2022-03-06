import 'dart:async';

import 'package:adaptive_theme/adaptive_theme.dart';
import 'package:flutter/material.dart';

abstract class AdaptiveState<T extends StatefulWidget> extends State<T> {
  bool darkMode = false;
  static final StreamController _controller = StreamController.broadcast();
  static Stream get onChange => _controller.stream;
  late StreamSubscription listener;

  @override
  void initState() {
    super.initState();
    darkMode = AdaptiveTheme
        .of(context)
        .mode
        .isDark;
    listener = onChange.listen((event) {
      setState(() {
        darkMode = event;
      });
    });
  }

  @override
  void dispose()
  {
    listener.cancel();
    super.dispose();
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
    _controller.add(darkMode);
  }
}
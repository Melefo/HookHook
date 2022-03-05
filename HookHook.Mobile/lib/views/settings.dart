import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

class SettingsView extends StatefulWidget {
  const SettingsView({Key? key}) : super(key: key);

  static String routeName = "/settings";

  @override
  _Settings createState() => _Settings();
}

class _Settings extends AdaptiveState<SettingsView> {
  @override
  Widget build(BuildContext context) =>
      const Scaffold();
}
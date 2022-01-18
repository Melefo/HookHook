import "package:hookhook/views/home.dart";
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:mvc_application/src/view/app_state.dart';
import 'package:mvc_application/view.dart'
    show AppMVC;
import 'package:mvc_pattern/mvc_pattern.dart';

void main() async {
  runApp(Hookhook());
}

class Hookhook extends AppMVC {
  Hookhook({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) =>
      MaterialApp(
          debugShowCheckedModeBanner: kReleaseMode,
          title: "HookHook",
          theme: ThemeData(
              primarySwatch: Colors.pink,
              floatingActionButtonTheme: FloatingActionButtonThemeData(
                  backgroundColor: Colors.blue
              ),
              outlinedButtonTheme: OutlinedButtonThemeData(
                  style: OutlinedButton.styleFrom(
                    primary: Colors.blue,
                  )
              )
          ),
          initialRoute: HomeView.routeName,
          routes: {
            HomeView.routeName: (context) => const HomeView(),
          }
      );

  @override
  AppState<AppStatefulWidgetMVC> createState() {
    // TODO: implement createState
    throw UnimplementedError();
  }
}
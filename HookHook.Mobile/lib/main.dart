import 'package:hookhook/hookhook_colors.dart';
import "package:hookhook/views/home.dart";
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:adaptive_theme/adaptive_theme.dart';
import 'package:hookhook/views/login.dart';
import 'package:hookhook/views/new_area.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:mvc_application/view.dart'
    show AppMVC, AppState, AppStatefulWidgetMVC;

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final backend = await Backend.init();
  runApp(Hookhook(backend: backend));
}

class Hookhook extends AppMVC {
  Hookhook({Key? key, required this.backend}) : super(key: key);

  Backend backend;

  @override
  Widget build(BuildContext context) =>
      AdaptiveTheme(
          light: ThemeData(
            fontFamily: 'Comfortaa',
            brightness: Brightness.light,
            primaryColor: HookHookColors.light,
          ),
          dark: ThemeData(
              fontFamily: 'Comfortaa',
              brightness: Brightness.dark,
              primaryColor: HookHookColors.dark
          ),
          initial: AdaptiveThemeMode.system,
          builder: (theme, dark) =>
              MaterialApp(
                  debugShowCheckedModeBanner: kReleaseMode,
                  title: "HookHook",
                  theme: theme,
                  darkTheme: dark,
                  initialRoute: HomeView.routeName,
                  routes: {
                    HomeView.routeName: (context) => const HomeView(),
                    NewAreaView.routeName: (context) => NewAreaView(),
                    LoginView.routeName: (context) => LoginView()
                  }
              )
      );

  @override
  AppState<AppStatefulWidgetMVC> createState() {
    // TODO: implement createState
    throw UnimplementedError();
  }
}
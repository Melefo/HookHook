import 'package:flutter_screenutil/flutter_screenutil.dart';
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
      ScreenUtilInit(
          designSize: const Size(360, 690),
          minTextAdapt: true,
          splitScreenMode: true,
          builder: () =>
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
                          initialRoute: LoginView.routeName,
                          routes: {
                            HomeView.routeName: (context) => const HomeView(),
                            NewAreaView.routeName: (context) => const NewAreaView(),
                            LoginView.routeName: (context) => const LoginView()
                          },
                          builder: (context, widget) {
                            ScreenUtil.setContext(context);
                            return MediaQuery(
                                data: MediaQuery.of(context).copyWith(
                                    textScaleFactor: 1.0),
                                child: widget!
                            );
                          }
                      )
              )
      );

  @override
  AppState<AppStatefulWidgetMVC> createState() {
    // TODO: implement createState
    throw UnimplementedError();
  }
}
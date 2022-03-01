import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/views/forgot_password.dart';
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
  String? token = await HookHook.storage.read(key: Backend.tokenKey);
  String? instance = await HookHook.storage.read(key: Backend.instanceKey);
  await Backend.init(token: token, instance: instance);
  runApp(HookHook());
}

class HookHook extends AppMVC {
  HookHook({Key? key}) : super(key: key);

  static const FlutterSecureStorage storage = FlutterSecureStorage();
  static Backend backend = Backend();

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
                          initialRoute: backend.signIn.token != null ? HomeView.routeName : LoginView.routeName,
                          routes: {
                            HomeView.routeName: (context) => const HomeView(),
                            NewAreaView.routeName: (context) => const NewAreaView(),
                            LoginView.routeName: (context) => const LoginView(),
                            ForgotPassword.routeName: (context) => const ForgotPassword()
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

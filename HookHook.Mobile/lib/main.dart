import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/views/confirm_password.dart';
import 'package:hookhook/views/forgot_password.dart';
import "package:hookhook/views/home.dart";
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:adaptive_theme/adaptive_theme.dart';
import 'package:hookhook/views/login.dart';
import 'package:hookhook/views/new_area.dart';
import 'package:hookhook/views/register.dart';
import 'package:hookhook/views/verify.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:mvc_application/view.dart'
    show AppMVC, AppState, AppStatefulWidgetMVC;
import 'package:flutter_dotenv/flutter_dotenv.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final savedThemeMode = await AdaptiveTheme.getThemeMode();
  await dotenv.load(fileName: ".env");
  String? token = await HookHook.storage.read(key: Backend.tokenKey);
  String? username = await HookHook.storage.read(key: Backend.usernameKey);
  String? password = await HookHook.storage.read(key: Backend.passwordKey);
  String? instance = await HookHook.storage.read(key: Backend.instanceKey);
  await Backend.init(instance, token, username, password);
  runApp(HookHook(token, savedThemeMode));
}

class HookHook extends AppMVC {
  final String? _token;
  final AdaptiveThemeMode? theme;
  HookHook(this._token, this.theme, {Key? key}) : super(key: key);

  static const FlutterSecureStorage storage = FlutterSecureStorage();
  static Backend backend = Backend();

  MaterialPageRoute? constructRoutes(RouteSettings settings) {
    if (settings.name!.startsWith(ConfirmPassword.routeName)) {
      var parts = settings.name!.split('/');
      if (parts.length == 3) {
        return MaterialPageRoute(
            builder: (context) => ConfirmPassword(id: parts[parts.length - 1])
        );
      }
    }
    if (settings.name!.startsWith(Verify.routeName)) {
      var parts = settings.name!.split('/');
      if (parts.length == 3) {
        return MaterialPageRoute(
            builder: (context) => Verify(id: parts[parts.length -1])
        );
      }
    }
    return null;
  }

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
                  initial: theme ?? AdaptiveThemeMode.system,
                  builder: (theme, dark) =>
                      MaterialApp(
                          debugShowCheckedModeBanner: kReleaseMode,
                          title: "HookHook",
                          theme: theme,
                          darkTheme: dark,
                          initialRoute: _token != null ? HomeView.routeName : LoginView.routeName,
                          routes: {
                            HomeView.routeName: (context) => const HomeView(),
                            NewAreaView.routeName: (context) => const NewAreaView(),
                            LoginView.routeName: (context) => const LoginView(),
                            ForgotPassword.routeName: (context) => const ForgotPassword(),
                            RegisterView.routeName: (context) => const RegisterView()
                          },
                          onGenerateRoute: (settings) =>
                              constructRoutes(settings),
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
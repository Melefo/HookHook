import "package:hookhook/views/home.dart";
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
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
      MaterialApp(
          debugShowCheckedModeBanner: kReleaseMode,
          title: "HookHook",
          theme: ThemeData(
              fontFamily: 'Comfortaa'
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
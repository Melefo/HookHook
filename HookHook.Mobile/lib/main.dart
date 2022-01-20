import "package:hookhook/views/home.dart";
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:mvc_application/view.dart'
    show AppMVC, AppState, AppStatefulWidgetMVC;

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
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
              primarySwatch: Colors.pink, //ToDo changer la couleur
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
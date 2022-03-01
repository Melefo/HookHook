import 'package:hookhook/wrapper/signIn.dart';

import 'about.dart';

class Backend {
  static Backend? _this;

  factory Backend() =>
      _this ??= Backend._();

  static String apiEndpoint = String.fromEnvironment(
      "BACKEND_URL",
      defaultValue: "http://localhost:8080/"
  );

  About? about;
  SignIn signIn = SignIn();

  Backend._();

  static Future<Backend> init() async
  {
    Backend backend = Backend();
    try {
      var about = await About.init();
      backend.about = about;
    }
    on Exception {
      print("Failed to call backend");
    }
    return backend;
  }
}
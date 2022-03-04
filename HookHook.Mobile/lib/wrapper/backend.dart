import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:hookhook/wrapper/area_client.dart';
import 'package:hookhook/wrapper/sign_in.dart';
import 'about.dart';

class Backend {
  static Backend? _this;
  static const String tokenKey = "user_token";
  static const String instanceKey = "instance";

  factory Backend() =>
      _this ??= Backend._();

  static String apiEndpoint = dotenv.env["BACKEND_URL"] ?? "http://localhost:8080/";

  About? about;
  SignIn signIn = SignIn();
  AreaClient area = AreaClient();

  Backend._();

  static Future<Backend> init({String? token, String? instance}) async
  {
    Backend backend = Backend();
    if (token != null) {
      backend.signIn.token = token;
    }
    if (instance != null) {
      Backend.apiEndpoint = instance;
    }
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
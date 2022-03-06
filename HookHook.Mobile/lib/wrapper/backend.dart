import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'sign_in.dart';
import 'about.dart' hide Service;
import 'area_client.dart';
import 'service.dart';

class Backend {
  static Backend? _this;
  static const String tokenKey = "user_token";
  static const String usernameKey = "user_name";
  static const String passwordKey = "user_password";
  static const String instanceKey = "instance";

  factory Backend([String? token, String? username, String? password]) =>
      _this ??= Backend._(token, username, password);

  static String apiEndpoint = dotenv.env["BACKEND_URL"] ?? "http://hookhook.xyz:8080/";

  About? about;
  SignIn signIn;
  AreaClient area = AreaClient();
  Service service = Service();

  Backend._([String? token, String? username, String? password]) : signIn = SignIn(token, username, password);

  static Future<Backend> init(String? instance, [String? token, String? username, String? password]) async
  {
    Backend backend = Backend(token, username, password);
    if (instance != null) {
      Backend.apiEndpoint = instance;
    }
    try {
      var about = await About.init();
      backend.about = about;
    }
    on Exception {
      if (kDebugMode) {
        print("Failed to call backend");
      }
    }
    return backend;
  }
}
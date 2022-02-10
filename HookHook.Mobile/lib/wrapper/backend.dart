import 'about.dart';

class Backend
{
  static const String apiEndpoint = String.fromEnvironment("BACKEND_URL", defaultValue: "http://localhost:8080/");

  late About about;

  Backend._();

  static Future<Backend> init() async
  {
    Backend backend = Backend._();
    backend.about = await About.init();
    return backend;
  }
}
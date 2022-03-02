import 'package:http/http.dart' as http;
import 'backend.dart';

class AreaClient {

  late List area;

  AreaClient._();

  static const String urlAll = "/area/all";

  static Future<String> updateAll() async {
    final res = await http.get(Uri.parse(Backend.apiEndpoint + urlAll)).timeout(const Duration(
        seconds: 3
    ));

    return ("Test");
    throw Exception();
  }
}
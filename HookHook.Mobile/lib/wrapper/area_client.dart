import 'dart:io';
import 'package:hookhook/main.dart';
import 'package:hookhook/models/area_model.dart';
import 'package:http/http.dart' as http;
import 'backend.dart';

class AreaClient {

  AreaClient();

  void deleteAreaFromID(String id) async {
    String url = "area/delete/" + id;
    print(url);
    final response = await http.delete(
      Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },
    );
    if (response.statusCode == 204) {
      var data = response.body;
      print(data);
    } else {
      throw Exception();
    }
  }

  void triggerAreaFromID(String id) async {
    String url = "area/trigger/" + id;
    print(url);
    final res = await http.get(Uri.parse(Backend.apiEndpoint + url)).timeout(const Duration(
        seconds: 3
    ));
    final response = await http.get(
      Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },
    );
    if (response.statusCode == 204) {
      var data = response.body;
      print(data);
    } else {
      throw Exception();
    }
  }

  Future<List<AreaModel>> fetchAreas() async {
    const String url = "area/all";

    final res = await http.get(Uri.parse(Backend.apiEndpoint + url)).timeout(const Duration(
      seconds: 3
    ));
    final response = await http.get(
      Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },
    );
    if (response.statusCode == 200) {
      var data = response.body;
      return areaModelFromJson(data);
    } else {
      throw Exception();
    }
  }
}
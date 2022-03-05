import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/models/area_model.dart';
import 'package:http/http.dart' as http;
import 'backend.dart';

class AreaClient {

  AreaClient();

  Future<void> deleteAreaFromID(String id) async {
    String url = "area/delete/" + id;
    final res = await http.delete(
      Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },
    );
    if (res.statusCode != 204 && kDebugMode) {
      print("FAILED TO UPDATE");
    }
  }

  Future<void> triggerAreaFromID(String id) async {
    String url = "area/trigger/" + id;
    final res = await http.get(Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },).timeout(const Duration(
        seconds: 3
    ));
    if (res.statusCode != 204 && kDebugMode) {
      print("FAILED TO UPDATE");
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
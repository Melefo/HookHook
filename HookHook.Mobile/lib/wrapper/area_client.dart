import 'dart:io';

import 'package:flutter/material.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/models/area_model.dart';
import 'package:http/http.dart' as http;
import 'backend.dart';

class AreaClient {

  AreaClient();

  Future<List<AreaModel>> fetchAreas() async {
    const String url = "area/all";

    final res = await http.get(Uri.parse(Backend.apiEndpoint + url)).timeout(const Duration(
      seconds: 3
    ));
    final response = await http.get(
      Uri.parse(Backend.apiEndpoint + url),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + HookHook.backend.signIn.token!,
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
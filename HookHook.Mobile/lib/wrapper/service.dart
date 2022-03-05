import 'dart:convert';
import 'dart:io';

import '../main.dart';
import 'backend.dart';
import 'package:http/http.dart' as http;

class Account {
  Account({
    required this.userId,
    required this.username,
  });

  final String userId;
  final String username;

  factory Account.fromJson(Map<String, dynamic> json) => Account(
    username: json["username"],
    userId: json["userId"],
  );

  Map<String, dynamic> toJson() => {
    "userId": userId,
    "username": username,
  };
}

class Service {
  static String baseUrl = Backend.apiEndpoint + "service/";

  Future<List<Account>> getAccounts(String provider) async {
    final res = await http.get(Uri.parse("$baseUrl$provider"),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " + (await HookHook.backend.signIn.token)!,
      },
    );
    if (res.statusCode == 200) {
      return List<Account>.from(jsonDecode(res.body));
    }
    return [];
  }
}
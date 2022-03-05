import 'dart:convert';
import 'dart:io';

import 'package:flutter/foundation.dart';

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
  static String twitterUrl = baseUrl + "twitter";

  Future<List<Account>> getAccounts(String provider) async {
    final res = await http.get(Uri.parse("$baseUrl$provider"),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " +
            (await HookHook.backend.signIn.token)!,
      },
    ).timeout(const Duration(
        seconds: 3
    ));
    if (res.statusCode == 200) {
      return List<Account>.from(
          jsonDecode(res.body).map((x) => Account.fromJson(x)));
    }
    return [];
  }

  Future<void> deleteAccount(String provider, String id) async {
    final res = await http.delete(Uri.parse("$baseUrl$provider?id=$id"),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " +
            (await HookHook.backend.signIn.token)!,
      },
    ).timeout(const Duration(
        seconds: 3
    ));
    if (res.statusCode != 204 && kDebugMode) {
      print("DELETE ACCOUNT FAILED");
    }
  }

  Future<Account?> addTwitter(String code, String verifier) async {
    final res = await http.post(Uri.parse(
        "$twitterUrl?code=$code&verifier=$verifier"),
      headers: {
        HttpHeaders.authorizationHeader: "Bearer " +
            (await HookHook.backend.signIn.token)!,
      },
    )
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      return Account.fromJson(jsonDecode(res.body));
    }
    return null;
  }
}
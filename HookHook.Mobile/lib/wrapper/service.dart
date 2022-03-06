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
  static String twitchUrl = baseUrl + "twitch";
  static String spotifyUrl = baseUrl + "spotify";
  static String googleUrl = baseUrl + "google";
  static String gitHubUrl = baseUrl + "github";
  static String discordUrl = baseUrl + "discord";

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
    if (res.statusCode != 204) {
      if (kDebugMode) {
        print("DELETE ACCOUNT FAILED");
      }
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

  Future<Account?> addTwitch(String code) async {
    final res = await http.post(Uri.parse(
        "$twitchUrl?code=$code"),
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

  Future<Account?> addSpotify(String code) async {
    final res = await http.post(Uri.parse(
        "$spotifyUrl?code=$code&redirect=hookhook://oauth/spotify"),
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

  Future<Account?> addGoogle(String code) async {
    final res = await http.post(Uri.parse(
        "$googleUrl?code=$code"),
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

  Future<Account?> addGitHub(String code) async {
    final res = await http.post(Uri.parse(
        "$gitHubUrl?code=$code"),
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

  Future<Account?> addDiscord(String code, String verifier) async {
    final res = await http.post(Uri.parse(
        "$discordUrl?code=$code&verifier=$verifier&redirect=hookhook://oauth/discord"),
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
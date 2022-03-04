import 'dart:convert';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:http/http.dart' as http;

import '../main.dart';

class _Login
{
  final Map<String, dynamic> _data;

  String get token => _data["token"];

  _Login._(this._data);
}

class SignIn {
  String? token;

  static String loginUrl = baseUrl + "login";
  static String spotifyUrl = baseUrl + "oauth/spotify";
  static String discordUrl = baseUrl + "oauth/discord";
  static String verifyUrl = baseUrl + "verify/";
  static String confirmUrl = baseUrl + "confirm";
  static String registerUrl = baseUrl + "register";
  static String forgotUrl = baseUrl + "forgot/";
  static String baseUrl = Backend.apiEndpoint + "signin/";

  Future<void> forgotPassword(String username) async =>
      await http.put(Uri.parse(forgotUrl + username)).timeout(const Duration(
          seconds: 3
      ));

  Future<void> login(String username, String password) async {
    final res = await http.post(Uri.parse(loginUrl),
        headers: <String, String>{
          'Content-Type': 'application/json',
        },
        body: jsonEncode(<String, String>{
          'username': username,
          'password': password
        })
    ).timeout(const Duration(
        seconds: 3
    ));
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }

  Future<void> verifyEmail(String id) async {
    final res = await http.put(Uri.parse(verifyUrl + id)).timeout(
        const Duration(
            seconds: 3
        ));
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }

  Future<void> confirmPassword(String id, String password) async {
    final res = await http.put(Uri.parse(confirmUrl),
        headers: <String, String>{
          'Content-Type': 'application/json'
        },
        body: jsonEncode(<String, String>{
          'password': password,
          'id': id
        })
    ).timeout(const Duration(
        seconds: 3
    ));
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }

  Future<void> register(String firstName, String lastName, String email,
      String username, String password) async {
    final res = await http.post(Uri.parse(registerUrl),
        headers: <String, String>{
          'Content-Type': 'application/json'
        },
        body: jsonEncode(<String, String>{
          'firstName': firstName,
          'lastName': lastName,
          'email': email,
          'username': username,
          'password': password
        })
    ).timeout(const Duration(
        seconds: 3
    ));
  }

  Future<void> discord(String code, String verifier) async {
    final res = await http.post(Uri.parse(
        "$discordUrl?code=$code&verifier=$verifier&redirect=${dotenv.env["DISCORD_REDIRECT"]!}"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }

  Future<void> spotify(String code) async {
    final res = await http.post(Uri.parse(
        "$spotifyUrl?code=$code&redirect=${dotenv.env["SPOTIFY_REDIRECT"]!}"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }
}
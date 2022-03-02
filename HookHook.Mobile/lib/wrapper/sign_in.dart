import 'dart:convert';

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
  static String forgotUrl = baseUrl + "forgot/";
  static String baseUrl = Backend.apiEndpoint + "signin/";

  Future<void> forgotPassword(String username) async =>
      await http.put(Uri.parse(forgotUrl + username));

  Future<void> login(String username, String password) async {
    final res = await http.post(Uri.parse(loginUrl),
        headers: <String, String>{
          'Content-Type': 'application/json',
        },
        body: jsonEncode(<String, String>{
          'username': username,
          'password': password
        })
    );
    if (res.statusCode == 200) {
      token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: token);
      await HookHook.storage.write(key: Backend.instanceKey, value: Backend.apiEndpoint);
    }
  }
}
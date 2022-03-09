import 'dart:convert';
import 'package:hookhook/wrapper/backend.dart';
import 'package:http/http.dart' as http;
import 'package:jwt_decode/jwt_decode.dart';

import '../main.dart';

class RegisterError {
  Errors? errors;

  RegisterError({this.errors});

  RegisterError.fromJson(Map<String, dynamic> json) {
    errors =
    json['errors'] != null ? Errors.fromJson(json['errors']) : null;
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = <String, dynamic>{};
    if (errors != null) {
      data['errors'] = errors!.toJson();
    }
    return data;
  }
}

class Errors {
  String? email;
  String? password;
  String? username;

  Errors({this.email, this.password, this.username});

  Errors.fromJson(Map<String, dynamic> json) {
    email = json['email'];
    password = json['password'];
    username = json['username'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = <String, dynamic>{};
    data['email'] = email;
    data['password'] = password;
    data['username'] = username;
    return data;
  }
}

class _Login
{
  final Map<String, dynamic> _data;

  String get token => _data["token"];
  _Login._(this._data);
}

class SignIn {
  String? _token;
  String? _username;
  String? _password;

  SignIn(this._token, this._username, this._password);

  Future<String?> get token async {
    if (_token != null && Jwt.isExpired(_token!)) {
      _token = null;
      if (_username != null && _password != null) {
          await login(_username!, _password!);
      }
    }

    return _token;
  }

  static String get loginUrl => baseUrl + "login";
  static String get spotifyUrl => baseUrl + "oauth/spotify";
  static String get googleUrl => baseUrl + "oauth/google";
  static String get githubUrl => baseUrl + "oauth/github";
  static String get discordUrl => baseUrl + "oauth/discord";
  static String get twitchUrl => baseUrl + "oauth/twitch";
  static String get twitterUrl => baseUrl + "oauth/twitter";
  static String get verifyUrl => baseUrl + "verify/";
  static String get confirmUrl => baseUrl + "confirm";
  static String get registerUrl => baseUrl + "register";
  static String get authorizeUrl => baseUrl + "authorize/";
  static String get forgotUrl => baseUrl + "forgot/";
  static String get baseUrl => Backend.apiEndpoint + "signin/";

  Future<void> forgotPassword(String username) async =>
      await http.put(Uri.parse(forgotUrl + username)).timeout(const Duration(
          seconds: 3
      ));

  Future<void> logout() async {
    _token = null;
    _username = null;
    _password = null;
    await HookHook.storage.delete(key: Backend.tokenKey);
    await HookHook.storage.delete(key: Backend.usernameKey);
    await HookHook.storage.delete(key: Backend.passwordKey);
    await HookHook.storage.delete(key: Backend.instanceKey);
  }

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
      _token = _Login
          ._(jsonDecode(res.body))
          .token;
      _username = username;
      _password = password;
      await HookHook.storage.write(key: Backend.tokenKey, value: await token);
      await HookHook.storage.write(key: Backend.usernameKey, value: _username);
      await HookHook.storage.write(key: Backend.passwordKey, value: _password);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
    else {
      throw http.ClientException("Invalid password");
    }
  }

  Future<void> verifyEmail(String id) async {
    final res = await http.put(Uri.parse(verifyUrl + id)).timeout(
        const Duration(
            seconds: 3
        ));
    if (res.statusCode == 200) {
      _token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: await token);
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
      _token = _Login
          ._(jsonDecode(res.body))
          .token;
      await HookHook.storage.write(key: Backend.tokenKey, value: await token);
      await HookHook.storage.write(
          key: Backend.instanceKey, value: Backend.apiEndpoint
      );
    }
  }

  Future<Errors?> register(String firstName, String lastName, String email,
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
    if (res.statusCode == 400) {
      return RegisterError.fromJson(jsonDecode(res.body)).errors;
    }
    return null;
  }

  Future<void> discord(String code, String verifier) async {
    final res = await http.post(Uri.parse(
        "$discordUrl?code=$code&verifier=$verifier&redirect=hookhook://oauth/discord"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> spotify(String code) async {
    final res = await http.post(Uri.parse(
        "$spotifyUrl?code=$code&redirect=hookhook://oauth/spotify"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> github(String code) async {
    final res = await http.post(Uri.parse(
        "$githubUrl?code=$code"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> google(String code) async {
    final res = await http.post(Uri.parse(
        "$googleUrl?code=$code"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> twitch(String code) async {
    final res = await http.post(Uri.parse(
        "$twitchUrl?code=$code"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> twitter(String code, String verifier) async {
    final res = await http.post(Uri.parse(
        "$twitterUrl?code=$code&verifier=$verifier"))
        .timeout(
        const Duration(
            seconds: 3
        )
    );
    if (res.statusCode == 200) {
      await saveToken(res.body);
    }
  }

  Future<void> saveToken(String body) async {
    _token = _Login
        ._(jsonDecode(body))
        .token;
    await HookHook.storage.write(key: Backend.tokenKey, value: await token);
    await HookHook.storage.write(
        key: Backend.instanceKey, value: Backend.apiEndpoint
    );
  }

  Future<String?> authorize(String provider, String redirect) async {
    final res = await http.get(Uri.parse(
      "$authorizeUrl$provider?redirect=$redirect"
    )).timeout(
      const Duration(
        seconds: 3
      )
    );
    if (res.statusCode == 200) {
      return res.body;
    }
    return null;
  }
}
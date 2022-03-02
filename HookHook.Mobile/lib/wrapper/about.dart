import 'dart:convert';
import 'package:http/http.dart' as http;

import 'backend.dart';

class Action
{
  final Map<String, dynamic> _data;

  String get name => _data["name"];
  String get description => _data["description"];

  Action._(this._data);
}

class Reaction
{
  final Map<String, dynamic> _data;

  String get name => _data["name"];
  String get description => _data["description"];

  Reaction._(this._data);
}

class Service
{
  final Map<String, dynamic> _data;

  String get name => _data["name"];

  late List<Action> actions = [];
  late List<Reaction> reactions = [];

  Service._(this._data)
  {
    for (var element in _data["actions"]) {
      actions.add(Action._(element));
    }
    for (var element in _data["reactions"]) {
      reactions.add(Reaction._(element));
    }
  }
}

class Server
{
  final Map<String, dynamic> _data;

  int get currentTime => _data["currentTime"];

  List<Service> services = [];

  Server._(this._data)
  {
    for (var element in _data["services"]) {
      services.add(Service._(element));
    }
  }
}

class Client
{
  final Map<String, dynamic> _data;

  String get host => _data["host"];

  Client._(this._data);
}

class About
{
  final Map<String, dynamic> _data;

  late Client client;
  late Server server;

  About._(this._data)
  {
    client = Client._(_data["client"]);
    server = Server._(_data["server"]);
  }

  static const String url = "about.json";

  static Future<About> init() async {
    final res = await http.get(Uri.parse(Backend.apiEndpoint + url)).timeout(const Duration(
      seconds: 3
    ));

    if (res.statusCode == 200) {
      return About._(jsonDecode(res.body));
    }
    throw Exception();
  }
}
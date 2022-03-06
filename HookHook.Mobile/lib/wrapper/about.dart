import 'dart:convert';
import 'package:http/http.dart' as http;

import 'backend.dart';

class Action {
  final String name;
  final String description;

  Action({
    required this.name,
    required this.description
  });

  factory Action.fromJson(Map<String, dynamic> json) =>
      Action(
        name: json["name"],
        description: json["description"],
      );

  Map<String, dynamic> toJson() =>
      {
        "name": name,
        "description": description,
      };
}

class Reaction {
  final String name;
  final String description;

  Reaction({
    required this.name,
    required this.description
  });

  factory Reaction.fromJson(Map<String, dynamic> json) =>
      Reaction(
        name: json["name"],
        description: json["description"],
      );

  Map<String, dynamic> toJson() =>
      {
        "name": name,
        "description": description,
      };
}

class Service {
  Service({
    required this.name,
    required this.actions,
    required this.reactions,
  });

  final String name;
  final List<Action> actions;
  final List<Reaction> reactions;

  factory Service.fromJson(Map<String, dynamic> json) =>
      Service(
        name: json["name"],
        actions: List<Action>.from(
            json["actions"].map((x) => Action.fromJson(x))),
        reactions: List<Reaction>.from(
            json["reactions"].map((x) => Reaction.fromJson(x))),
      );

  Map<String, dynamic> toJson() =>
      {
        "name": name,
        "actions": List<dynamic>.from(actions.map((x) => x.toJson())),
        "reactions": List<dynamic>.from(reactions.map((x) => x.toJson())),
      };
}

class Server {
  Server({
    required this.currentTime,
    required this.services,
  });

  final int currentTime;
  final List<Service> services;

  factory Server.fromJson(Map<String, dynamic> json) =>
      Server(
        currentTime: json["currentTime"],
        services: List<Service>.from(
            json["services"].map((x) => Service.fromJson(x))),
      );

  Map<String, dynamic> toJson() =>
      {
        "currentTime": currentTime,
        "services": List<dynamic>.from(services.map((x) => x.toJson())),
      };
}

class Client {
  Client({
    required this.host,
  });

  final String host;

  factory Client.fromJson(Map<String, dynamic> json) =>
      Client(
        host: json["host"],
      );

  Map<String, dynamic> toJson() =>
      {
        "host": host,
      };
}

class About
{
  final Client client;
  final Server server;

  About({
    required this.client,
    required this.server
  });

  factory About.fromJson(Map<String, dynamic> json) =>
      About(
        client: Client.fromJson(json["client"]),
        server: Server.fromJson(json["server"]),
      );

  Map<String, dynamic> toJson() =>
      {
        "client": client.toJson(),
        "server": server.toJson()
      };

  static const String url = "about.json";

  static Future<About> init() async {
    final res = await http.get(Uri.parse(Backend.apiEndpoint + url)).timeout(const Duration(
      seconds: 3
    ));

    if (res.statusCode == 200) {
      return About.fromJson(jsonDecode(res.body));
    }
    throw Exception();
  }
}
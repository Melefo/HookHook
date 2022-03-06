// To parse this JSON data, do
//
//     final createAreaModel = createAreaModelFromJson(jsonString);

import 'package:meta/meta.dart';
import 'dart:convert';

CreateAreaModel createAreaModelFromJson(String str) => CreateAreaModel.fromJson(json.decode(str));

String createAreaModelToJson(CreateAreaModel data) => json.encode(data.toJson());

class CreateAreaModel {
  CreateAreaModel({
    required this.action,
    required this.reactions,
    required this.minutes,
    required this.name,
  });

  final Action action;
  final List<Action> reactions;
  final int minutes;
  final String name;

  factory CreateAreaModel.fromJson(Map<String, dynamic> json) => CreateAreaModel(
    action: Action.fromJson(json["action"]),
    reactions: List<Action>.from(json["reactions"].map((x) => Action.fromJson(x))),
    minutes: json["minutes"],
    name: json["name"],
  );

  Map<String, dynamic> toJson() => {
    "action": action.toJson(),
    "reactions": List<dynamic>.from(reactions.map((x) => x.toJson())),
    "minutes": minutes,
    "name": name,
  };
}

class Action {
  Action({
    required this.type,
    required this.arguments,
    required this.accountId,
  });

  final String type;
  final List<String> arguments;
  final String accountId;

  factory Action.fromJson(Map<String, dynamic> json) => Action(
    type: json["type"],
    arguments: List<String>.from(json["arguments"].map((x) => x)),
    accountId: json["accountId"],
  );

  Map<String, dynamic> toJson() => {
    "type": type,
    "arguments": List<dynamic>.from(arguments.map((x) => x)),
    "accountId": accountId,
  };
}

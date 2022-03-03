// To parse this JSON data, do
//
//     final areaModel = areaModelFromJson(jsonString);

import 'dart:convert';

List<AreaModel> areaModelFromJson(String str) => List<AreaModel>.from(json.decode(str).map((x) => AreaModel.fromJson(x)));

String areaModelToJson(List<AreaModel> data) => json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class AreaModel {
  AreaModel({
    required this.id,
    required this.name,
    required this.from,
    required this.to,
    required this.date,
  });

  final String id;
  final String name;
  final String from;
  final List<String> to;
  final int date;

  factory AreaModel.fromJson(Map<String, dynamic> json) => AreaModel(
    id: json["id"],
    name: json["name"],
    from: json["from"],
    to: List<String>.from(json["to"].map((x) => x)),
    date: json["date"],
  );

  Map<String, dynamic> toJson() => {
    "id": id,
    "name": name,
    "from": from,
    "to": List<dynamic>.from(to.map((x) => x)),
    "date": date,
  };
}

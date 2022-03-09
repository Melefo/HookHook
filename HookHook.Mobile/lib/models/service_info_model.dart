// To parse this JSON data, do
//
//     final servicesInfoModel = servicesInfoModelFromJson(jsonString);

import 'dart:convert';

List<ServicesInfoModel> servicesInfoModelFromJson(String str) => List<ServicesInfoModel>.from(json.decode(str).map((x) => ServicesInfoModel.fromJson(x)));

String servicesInfoModelToJson(List<ServicesInfoModel> data) => json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class ServicesInfoModel {
  ServicesInfoModel({
    required this.name,
    required this.className,
    required this.description,
    required this.parameterNames,
    required this.formatters,
    required this.areaType,
  });

  final String name;
  final String className;
  final String description;
  final List<String> parameterNames;
  final List<String> ?formatters;
  final AreaType areaType;

  factory ServicesInfoModel.fromJson(Map<String, dynamic> json) => ServicesInfoModel(
    name: json["name"],
    className: json["className"],
    description: json["description"],
    parameterNames: List<String>.from(json["parameterNames"].map((x) => x)),
    formatters: json["formatters"] == null ? null : List<String>.from(json["formatters"].map((x) => x)),
    areaType: areaTypeValues.map[json["areaType"]]!,
  );

  Map<String, dynamic> toJson() => {
    "name": name,
    "className": className,
    "description": description,
    "parameterNames": List<dynamic>.from(parameterNames.map((x) => x)),
    "formatters": formatters == null ? null : List<dynamic>.from(formatters!.map((x) => x)),
    "areaType": areaTypeValues.reverse[areaType],
  };
}

enum AreaType { reaction, action }

final areaTypeValues = EnumValues({
  "Action": AreaType.action,
  "Reaction": AreaType.reaction
});

class EnumValues<T> {
  Map<String, T> map;
  Map<T, String> ?reverseMap;

  EnumValues(this.map);

  Map<T, String> get reverse {
    reverseMap ??= map.map((k, v) => MapEntry(v, k));
    return reverseMap!;
  }
}

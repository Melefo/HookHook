import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/wrapper/service.dart';
import 'package:hookhook/wrapper/about.dart' as about;


class Choices {
  Choices ();

  String? service;
  String? userId;
  String? action;
  List<String>? args;
}

class ActionParameters {
  ActionParameters ();

  List<ServicesInfoModel>? events;
  List<Account>? accounts;
  List<String>? user;
  Choices choice = Choices();
}
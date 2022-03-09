import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/wrapper/service.dart';

class Choices {
  Choices ();

  String? service;
  String? userId;
  String? action;
  List<String>? args;
}

class ActionParameters {
  ActionParameters();

  List<ServicesInfoModel>? events;
  List<Account>? accounts;
  List<String>? user;
  Choices choice = Choices();

  bool validate() =>
      choice.service != null && choice.userId != null &&
          choice.action != null && choice.args != null &&
          choice.args!.every((element) => element.isNotEmpty);
}
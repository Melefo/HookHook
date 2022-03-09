import 'package:flutter/material.dart';
import 'package:flutter_spinbox/flutter_spinbox.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/create_area_model.dart' as create;
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/widgets/creator_events.dart';
import 'package:hookhook/widgets/creator_services.dart';
import 'package:hookhook/widgets/creator_users.dart';
import 'package:hookhook/widgets/services_icon.dart';
import 'package:hookhook/wrapper/about.dart' as about;
import 'package:hookhook/wrapper/area_client.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:hookhook/wrapper/service.dart';
import '../adaptive_state.dart';
import '../hookhook_colors.dart';

class NewAreaView extends StatefulWidget {
  const NewAreaView({Key? key}) : super(key: key);

  static String routeName = "/new_area";

  @override
  _NewAreaView createState() => _NewAreaView();
}

class _NewAreaView extends AdaptiveState<NewAreaView> {

  int step = 0;
  int refreshTime = 5;
  TextEditingController areaName = TextEditingController(
      text: "Default Area Name");
  List<TextEditingController> fromArg = [];
  List<TextEditingController> toArg = [];
  List<about.Service> services = Backend().about!.server.services;
  String from = "from_null";
  String fromUser = "fromUser_null";
  String fromAction = "fromAction_null";
  String to = "to_null";
  String toUser = "toUser_null";
  String toAction = "toAction_null";
  List<String> toArgs = [];
  List<Account> accountAction = [];
  List<Account> accountReaction = [];
  List<ServicesInfoModel> possibleAction = [];
  List<ServicesInfoModel> possibleReaction = [];
  ///
  List<ServicesInfoModel>? actionEvents;
  String? actionUser;
  String? chosenAction;
  ///
  ActionParameters action = ActionParameters();
  ActionParameters reactions = ActionParameters();
  ///

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          resizeToAvoidBottomInset: false,
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors.light,
          body: Padding(
            padding: const EdgeInsets.fromLTRB(
                16, 30, 16, 20),
            child: Container(
              child: Column(
                children: [
                  CreatorServices(
                    services: services,
                    onUpdate: (accounts, servicesInfo, chosenService) {
                      setState(() {
                        action.events = servicesInfo;
                        action.accounts = accounts;
                        action.choice.service = chosenService;
                      });
                    }
                  ),
                  if (action.choice.service != null && action.accounts != null) CreatorUsers(
                      acc: action.accounts!,
                      onUpdate: (user) {
                        setState(() {
                          action.choice.userId = user;
                        });
                      }
                  ),
                  if (action.events != null && action.choice.service != null && action.choice.userId != null) CreatorEvents(
                      events: action.events!,
                      chosenService: action.choice.service!,
                      areaType: AreaType.ACTION,
                      onUpdate: (chosenAction) {
                        action.choice.action = chosenAction;
                      },
                    ),
                  Padding(
                    padding: const EdgeInsets.only(top: 20),
                    child: Center(
                      child: Container(
                        decoration: BoxDecoration(
                            borderRadius: BorderRadius.all(Radius.circular(25)),
                            color: darkMode ? HookHookColors.gray : Colors.white,
                        ),
                        child: TextButton(
                          onPressed: () {
                            Navigator.pop(context);
                          },
                          child: Padding(
                            padding: const EdgeInsets.fromLTRB(
                                120.0, 15.0, 120.0, 15.0),
                            child: Text(
                              "Back",
                              style: TextStyle(
                                  fontSize: 20,
                                  color: darkMode ? Colors.white : HookHookColors.dark,
                                  fontWeight: FontWeight.w500),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  SingleChildScrollView(
                    child: SizedBox(
                      height: 600,
                      child: Column(
                        children: [
                          Padding(
                            padding: const EdgeInsets.all(10.0),
                            child: TextFormField(
                              decoration: const InputDecoration(
                                  border: UnderlineInputBorder(),
                                  labelText: "Area Name"
                              ),
                              controller: areaName,
                            ),
                          ),

                          /// ACTION PART
                          Padding(
                            padding: const EdgeInsets.only(top: 20.0),
                            child: Align(
                                alignment: Alignment.centerLeft,
                                child: Text(
                                    "When :",
                                    style: TextStyle(
                                      color: darkMode ? HookHookColors.blue : HookHookColors.orange
                                    ),
                                )
                            ),
                          ),
                          DropdownButton(
                            value: from,
                            items: <DropdownMenuItem>[
                              const DropdownMenuItem(
                                value: "from_null",
                                child: Text("Choose your Service"),
                              ),
                              for (about.Service element in services) DropdownMenuItem(
                                value: "from_" + element.name,
                                child: Row(
                                  children: [
                                    ServiceIcon.serviceIcon(element.name),
                                    Text(element.name),
                                  ],
                                ),
                              )
                            ],
                            onChanged: (dynamic value) async {
                              if (value != "from_null") {
                                step = 1;
                              } else {
                                step = 0;
                              }
                              from = value!.toString();
                              fromUser = "fromUser_null";
                              fromAction = "fromAction_null";
                              final action = await Backend().service.getAccounts(
                                  (from.replaceAll("from_", "")));
                              setState(() {
                                if (value != "from_null") {
                                  accountAction = action;
                                }
                              });
                            },
                          ),
                          //
                          Visibility(
                              visible: step > 0 ? true : false,
                              child: DropdownButton(
                                value: fromUser,
                                items: <DropdownMenuItem>[
                                  for (Account account in accountAction) DropdownMenuItem(
                                    value: "from_" +  account.userId,
                                    child: Text(account.username),
                                  ),
                                  const DropdownMenuItem(
                                    value: "fromUser_null",
                                    child: Text('Choose an User'),
                                  ),
                                ],
                                onChanged: (dynamic value) async {
                                  if (value != "fromUser_null") {
                                    step = 2;
                                  } else {
                                    step = 1;
                                  }
                                  fromUser = value!.toString();
                                  fromAction = "fromAction_null";
                                  final action = await Backend().area.getServices();
                                  setState(() {
                                    if (value != "fromUser_null") {
                                      possibleAction = action.where((element) =>
                                      element.name.toLowerCase() ==
                                          from.replaceAll("from_", "").toLowerCase() &&
                                          element.areaType == AreaType.ACTION).toList();
                                    }
                                  });
                                },
                              )
                          ),
                          //
                          Visibility(
                              visible: step > 1 ? true : false,
                              child: DropdownButton(
                                value: fromAction,
                                items: <DropdownMenuItem>[
                                  for (ServicesInfoModel service in possibleAction) DropdownMenuItem(
                                    value: service.className,
                                    child: Text(service.description),
                                  ),
                                  const DropdownMenuItem(
                                    value: "fromAction_null",
                                    child: Text('Choose an Action'),
                                  ),
                                ],
                                onChanged: (dynamic value) {
                                  if (value != "fromAction_null") {
                                    step = 3;
                                  } else {
                                    step = 2;
                                  }

                                  final areas = Backend();
                                  setState(() {
                                    fromArg = [];
                                    for (String elem in possibleAction
                                        .singleWhere((element) =>
                                    element.className == value)
                                        .parameterNames) {
                                      fromArg.add(
                                          TextEditingController()
                                      );
                                    }
                                    fromAction = value!.toString();
                                  });
                                },
                              )
                          ),
                          Visibility(
                              visible: step > 1 ? true : false,
                              child: Column(
                                children: [
                                  if (possibleAction.isNotEmpty &&
                                      fromAction != "fromAction_null")
                                    ...possibleAction
                                        .singleWhere((element) =>
                                    element.className == fromAction)
                                        .parameterNames
                                        .asMap()
                                        .entries
                                        .map((e) =>
                                        TextFormField(
                                          decoration: InputDecoration(
                                              border: UnderlineInputBorder(),
                                              labelText: e.value
                                          ),
                                          controller: fromArg[e.key],
                                        ),
                                    ).toList()
                                ],
                              )
                          ),

                          ///REACTION PART
                          Visibility(
                            visible: step > 2 ? true : false,
                            child: Column(
                              children: [
                                const Divider(),
                                Padding(
                                  padding: const EdgeInsets.only(top: 25.0),
                                  child: Align(
                                      alignment: Alignment.centerLeft,
                                      child: Text(
                                        "Do :",
                                        style: TextStyle(
                                            color: darkMode ? HookHookColors.blue : HookHookColors.orange
                                        ),
                                      )
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Visibility(
                              visible: step > 2 ? true : false,
                              child: DropdownButton(
                                value: to,
                                items: <DropdownMenuItem>[
                                  for (about
                                      .Service element in services) DropdownMenuItem(
                                    value: "to_" + element.name,
                                    child: Row(
                                      children: [
                                        ServiceIcon.serviceIcon(element.name),
                                        Text(element.name),
                                      ],
                                    ),
                                  ),
                                  const DropdownMenuItem(
                                    value: "to_null",
                                    child: Text(""),
                                  ),
                                ],
                                onChanged: (dynamic value) async {
                                  if (value != "to_null") {
                                    step = 4;
                                  } else {
                                    step = 3;
                                  }
                                  to = value!.toString();
                                  final action = await Backend().service.getAccounts(
                                      (to.replaceAll("to_", "")));

                                  setState(() {
                                    if (value != "to_null") {
                                      accountReaction = action;
                                    }
                                  });
                                },
                              )
                          ),
                          Visibility(
                              visible: step > 3 ? true : false,
                              child: DropdownButton(
                                value: toUser,
                                items: <DropdownMenuItem>[
                                  for (Account account in accountReaction) DropdownMenuItem(
                                    value: account.userId,
                                    child: Text(account.username),
                                  ),
                                  const DropdownMenuItem(
                                    value: "toUser_null",
                                    child: Text('Choose an User'),
                                  ),
                                ],
                                onChanged: (dynamic value) async {
                                  if (value != "toUser_null") {
                                    step = 5;
                                  } else {
                                    step = 4;
                                  }
                                  toUser = value!.toString();
                                  final action = await Backend().area.getServices();

                                  setState(() {
                                    if (value != "toUser_null") {
                                      possibleReaction = action.where((element) =>
                                      element.name.toLowerCase() ==
                                          to.replaceAll("to_", "").toLowerCase() &&
                                          element.areaType == AreaType.REACTION).toList();
                                    }
                                  });
                                },
                              )
                          ),
                          //
                          Visibility(
                              visible: step > 4 ? true : false,
                              child: DropdownButton(
                                value: toAction,
                                items: <DropdownMenuItem>[
                                  const DropdownMenuItem(
                                    value: "toAction_null",
                                    child: Text('Choose a Reaction'),
                                  ),
                                  for (ServicesInfoModel service in possibleReaction) DropdownMenuItem(
                                    value: service.className,
                                    child: Text(service.description),
                                  ),
                                ],
                                onChanged: (dynamic value) {
                                  if (value != "fromAction_null") {
                                    step = 6;
                                  } else {
                                    step = 5;
                                  }
                                  setState(() {
                                    toArg = [];
                                    for (String elem in possibleReaction
                                        .singleWhere((element) =>
                                    element.className == value)
                                        .parameterNames)
                                      toArg.add(
                                          TextEditingController()
                                      );
                                    toAction = value!.toString();
                                  });
                                },
                              )
                          ),
                          Visibility(
                              visible: step > 5 ? true : false,
                              child: Column(
                                children: [
                                  if (possibleReaction.isNotEmpty &&
                                      toAction != "toAction_null")
                                    ...possibleReaction
                                        .singleWhere((element) =>
                                    element.className == toAction)
                                        .parameterNames
                                        .asMap()
                                        .entries
                                        .map((e) =>
                                        TextFormField(
                                          decoration: InputDecoration(
                                              border: UnderlineInputBorder(),
                                              labelText: e.value
                                          ),
                                          controller: toArg[e.key],
                                        ),
                                    ).toList()
                                ],
                              )
                          ),
                          Visibility(
                              visible: step > 5 ? true : false,
                              child: SpinBox(
                                min: 1,
                                max: 60,
                                value: 5,
                                onChanged: (value) {
                                  setState(() {
                                    refreshTime = value.toInt();
                                  });
                                },
                              )
                          ),
                        ],
                      ),
                    ),
                  ),
                  ///BUTTON CREATION
                  Visibility(
                      visible: step > 5 ? true : false,
                      child: Padding(
                        padding: const EdgeInsets.only(top: 20),
                        child: Center(
                          child: Container(
                            decoration: BoxDecoration(
                                borderRadius: BorderRadius.all(
                                    Radius.circular(25)),
                                color: darkMode ? HookHookColors.gray : Colors.white,
                            ),
                            child: TextButton(
                              onPressed: () async {
                                await Backend().area.createAreas(
                                    create.CreateAreaModel(action: create.Action(
                                        type: fromAction.replaceAll("from_", ""),
                                        arguments: fromArg.map((e) =>
                                        e.value.text).toList(),
                                        accountId: fromUser.replaceAll("from_", "")), reactions: [
                                      create.Action(type: toAction.replaceAll("to_", ""),
                                          arguments: toArg.map((e) =>
                                          e.value.text).toList(),
                                          accountId: toUser.replaceAll("to_", ""))
                                    ], minutes: refreshTime, name: areaName.value.text));
                                setState(() {});
                                Navigator.pop(context);
                              },
                              child: Padding(
                                padding: EdgeInsets.fromLTRB(
                                    120.0, 15.0, 120.0, 15.0),
                                child: Text(
                                  "New Area",
                                  style: TextStyle(
                                      fontSize: 20,
                                      color: darkMode ? Colors.white : HookHookColors.dark,
                                      fontWeight: FontWeight.w500),
                                ),
                              ),
                            ),
                          ),
                        ),
                      )
                  ),
                ],
              ),
            ),
          )
      );
}
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/create_area_model.dart' as create;
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/widgets/creator.dart';
import 'package:hookhook/wrapper/about.dart' as about;
import 'package:hookhook/wrapper/backend.dart';
import '../adaptive_state.dart';
import '../hookhook_colors.dart';

class NewAreaView extends StatefulWidget {
  const NewAreaView({Key? key}) : super(key: key);

  static String routeName = "/new_area";

  @override
  _NewAreaView createState() => _NewAreaView();
}

class _NewAreaView extends AdaptiveState<NewAreaView> {

  int refreshTime = 5;
  TextEditingController areaName = TextEditingController(
      text: "Default Area Name"
  );
  List<about.Service> services = Backend().about!.server.services;

  final ActionParameters action = ActionParameters();

  List<ActionParameters> reactions = [ ActionParameters() ];

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          resizeToAvoidBottomInset: false,
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors
              .light,
          body: Padding(
            padding: const EdgeInsets.fromLTRB(
                16, 30, 16, 20
            ),
            child: Column(
              children: [
                Padding(
                  padding: const EdgeInsets.only(top: 20),
                  child: Center(
                    child: Container(
                      decoration: BoxDecoration(
                        borderRadius: const BorderRadius.all(
                            Radius.circular(25)),
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
                                color: darkMode
                                    ? Colors.white
                                    : HookHookColors.dark,
                                fontWeight: FontWeight.w500),
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.only(top: 15.0),
                  child: Container(
                    padding: const EdgeInsets.all(8.0),
                    decoration: BoxDecoration(
                      borderRadius: const BorderRadius.all(
                          Radius.circular(25)
                      ),
                      border: Border.all(
                        width: 3.0,
                        color: darkMode ? Colors.white : HookHookColors.dark
                      )
                    ),
                    child: SizedBox(
                      height: 0.69.sh,
                      child: SingleChildScrollView(
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
                                        color: darkMode
                                            ? HookHookColors.blue
                                            : HookHookColors.orange
                                    ),
                                  )
                              ),
                            ),
                            Creator(
                              areaType: AreaType.action,
                              services: services,
                              action: action,
                              onUpdate: () {
                                setState(() {

                                });
                              },
                            ),

                            ///REACTION PART
                            Column(
                              children: [
                                const Divider(),
                                Padding(
                                  padding: const EdgeInsets.only(top: 25.0),
                                  child: Align(
                                      alignment: Alignment.centerLeft,
                                      child: Text(
                                        "Do :",
                                        style: TextStyle(
                                            color: darkMode ? HookHookColors
                                                .blue : HookHookColors.orange
                                        ),
                                      )
                                  ),
                                ),
                              ],
                            ),
                            for (int i = 0; i < reactions.length; i++)
                              Creator(
                                areaType: AreaType.reaction,
                                services: services,
                                action: reactions[i],
                                onUpdate: () {
                                  setState(() {

                                  });
                                },
                                onDelete: i == 0 ? null : () {
                                  setState(() {
                                    reactions.remove(reactions[i]);
                                  });
                                },
                              ),
                            TextButton(
                              onPressed: () {
                                setState(() {
                                  reactions.add(ActionParameters());
                                });
                              },
                              style: TextButton.styleFrom(
                                  backgroundColor: darkMode
                                      ? HookHookColors.gray
                                      : Colors.white,
                                  padding: const EdgeInsets.all(12),
                                  minimumSize: Size.zero,
                                  shape: RoundedRectangleBorder(
                                      borderRadius: BorderRadius.circular(10)
                                  )
                              ),
                              clipBehavior: Clip.none,
                              child: Icon(
                                  Icons.add,
                                  color: darkMode
                                      ? Colors.white
                                      : HookHookColors.gray
                              ),
                            )
                          ],
                        ),
                      ),
                    ),
                  ),
                ),

                ///BUTTON CREATION
                Visibility(
                    visible: action.validate() && reactions.every((element) => element.validate()),
                    child: Padding(
                      padding: const EdgeInsets.only(top: 20),
                      child: Center(
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: const BorderRadius.all(
                                Radius.circular(25)
                            ),
                            color: darkMode ? HookHookColors.gray : Colors
                                .white,
                          ),
                          child: TextButton(
                            onPressed: () async {
                              await Backend().area.createAreas(
                                  create.CreateAreaModel(
                                      action: create.Action(
                                        type: action.choice.action!,
                                        arguments: action.choice.args!,
                                        accountId: action.choice.userId!,
                                      ),
                                      reactions: reactions.map((e) =>
                                          create.Action(
                                              type: e.choice.action!,
                                              arguments: e.choice.args!,
                                              accountId: e.choice.userId!
                                          )).toList(),
                                      minutes: refreshTime,
                                      name: areaName.value.text
                                  ));
                              setState(() {});
                              Navigator.pop(context);
                            },
                            child: Padding(
                              padding: const EdgeInsets.fromLTRB(
                                  120.0, 15.0, 120.0, 15.0
                              ),
                              child: Text(
                                "New Area",
                                style: TextStyle(
                                    fontSize: 20,
                                    color: darkMode
                                        ? Colors.white
                                        : HookHookColors.dark,
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
          )
      );
}
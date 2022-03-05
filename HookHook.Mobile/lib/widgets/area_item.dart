import 'package:flutter/material.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

import '../services_icons.dart';

class AreaItem extends StatelessWidget {

  final String areaName;
  final String datetime;
  final String from;
  final String areaId;
  final List<String> to;

  const AreaItem(
      {Key? key, this.areaName = "Area Name", required this.areaId, this.datetime = "dateTime", this.from = "Action", this.to = const ["Reactions"]})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Container(
          alignment: Alignment.topCenter,
          padding: const EdgeInsets.all(10.0),
          height: 180,
          width: 350,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.all(Radius.circular(25)),
            color: Colors.white,
          ),
          child: Column(
            children: [
              Text(areaName, style: TextStyle(color: Colors.black)),
              Padding(
                padding: const EdgeInsets.only(top: 30),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    ServicesIcons.custom(from, 20),
                    Icon(Icons.arrow_right_alt_rounded, color: Colors.black),
                    for (String elem in to) ServicesIcons.custom(elem, 20),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(top: 20, right: 5, left: 5),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: <Widget>[
                    Text(datetime,
                        style: TextStyle(color: Colors.black)),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(top: 6),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    Container(
                      decoration: BoxDecoration(
                          borderRadius: BorderRadius.all(Radius.circular(10)),
                          color: HookHookColors.light
                      ),
                      child: IconButton(
                        onPressed: () {
                          print("trigger");
                          Backend().area.triggerAreaFromID(areaId);
                        },
                        icon: Icon(Icons.refresh, color: Colors.black),
                      ),
                    ),
                    Container(
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.all(Radius.circular(10)),
                        color: HookHookColors.light,
                      ),
                      child: IconButton(
                        onPressed: () {
                          showDialog<String>(
                            context: context,
                            builder: (BuildContext context) => AlertDialog(
                              content: const Text('You will delete this area'),
                              actions: <Widget>[
                                TextButton(
                                  onPressed: () => Navigator.pop(context, 'Cancel'),
                                  child: Text('Cancel', style: TextStyle(color: HookHookColors.orange)),
                                ),
                                TextButton(
                                  onPressed: () => Backend().area.deleteAreaFromID(areaId),
                                  child: Text('Yes', style: TextStyle(color: HookHookColors.blue)),
                                ),
                              ],
                            )
                          );
                        },
                        icon: Icon(Icons.delete, color: Colors.black),
                      ),
                    )
                  ],
                ),
              ),

            ],
          ),
        ),
      ),
    );
  }

}
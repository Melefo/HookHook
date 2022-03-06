import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/models/area_model.dart';
import 'package:intl/intl.dart';
import 'package:hookhook/wrapper/backend.dart';

import '../main.dart';
import 'area_item.dart';

class AreaList extends StatefulWidget {
  const AreaList({Key? key}) : super(key: key);

  static String routeName = "/login";

  @override
  _AreaList createState() => _AreaList();
}

class _AreaList extends AdaptiveState<AreaList> {

  List<AreaModel> areas = [];
  final DateFormat format = DateFormat('dd MMMM yyyy HH:mm');

  @override
  void initState() {
    Backend().area.fetchAreas().then((value) =>
        setState(() =>
        areas = value
        ));
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: ListView(
        padding: const EdgeInsets.all(0),
        scrollDirection: Axis.vertical,
        children: <Widget>[
          Column(
            children: [
              for (AreaModel elem in areas) AreaItem(
                areaName: elem.name,
                areaId: elem.id,
                datetime: format.format(
                    DateTime.fromMillisecondsSinceEpoch(elem.date * 1000)),
                from: elem.from,
                to: elem.to,
                trigger: () async {
                  await HookHook.backend.area.triggerAreaFromID(elem.id);
                  setState(() {
                    elem.date = (DateTime.now().millisecondsSinceEpoch / 1000).round();
                  });
                },
                delete: () async {
                  await HookHook.backend.area.deleteAreaFromID(elem.id);
                  setState(() {
                    areas.remove(elem);
                  });
                },
                failed: elem.lastLaunchFailed
              ),
            ],
          ),
        ],
      ),
    );
  }
}
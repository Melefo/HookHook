import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/views/new_area.dart';
import 'package:hookhook/widgets/area_list.dart';
import 'package:hookhook/widgets/hookhook_title.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/widgets/h_list.dart';
import 'package:hookhook/widgets/services_list.dart';

import '../adaptive_state.dart';

//view
class HomeView extends StatefulWidget {

  static String routeName = "/";

  const HomeView({Key? key}) : super(key: key);

  @override
  _Home createState() => _Home();
}

//state
class _Home extends AdaptiveState<HomeView> {

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors.light,
          body: Column(
            children: <Widget>[
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: HookHookTitle(welcome: true),
              ),
              const Align(
                alignment: Alignment.centerLeft,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(25, 10, 0, 10),
                  child: Text('Services', style: TextStyle(fontSize: 15)),
                ),
              ),
              HList(height: 0.07.sh, widget: ServicesList(itemWidth: 0.12.sw)),
              Container(
                decoration: BoxDecoration(
                    borderRadius: const BorderRadius.all(Radius.circular(25)),
                    color: darkMode ? HookHookColors.gray : Colors.white
                ),
                child: TextButton(
                  onPressed: () async {
                    await Navigator.pushNamed(context, NewAreaView.routeName);
                  },
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(120.0, 15.0, 120.0, 15.0),
                    child: Text(
                      "New Area",
                      style: TextStyle(
                          fontSize: 20,
                          color: darkMode ? Colors.white : Colors.black,
                          fontWeight: FontWeight.w500),
                    ),
                  ),
                ),
              ),
              const Align(
                alignment: Alignment.centerLeft,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(25, 20, 0, 20),
                  child: Text('Your AREAs', style: TextStyle(fontSize: 15)),
                ),
              ),
              const AreaList()
            ],
          ),
      );
}
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/models/area_model.dart';
import 'package:hookhook/views/new_area.dart';
import 'package:hookhook/widgets/area_item.dart';
import 'package:hookhook/widgets/area_list.dart';
import 'package:hookhook/wrapper/area_client.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/widgets/h_list.dart';
import 'package:hookhook/widgets/services_list.dart';

//view
class HomeView extends StatefulWidget {

  static String routeName = "/";

  const HomeView({Key? key}) : super(key: key);

  @override
  StateMVC<HomeView> createState() => _Home();
}

//state
class _Home extends StateMVC<HomeView> {

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: HookHookColors.light,
          body: Column(
            children: <Widget>[
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: Text('HookHook', textAlign: TextAlign.left,style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45)),
              ),
              const Align(
                alignment: Alignment.centerLeft,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(25, 30, 0, 10),
                  child: Text('Services', style: TextStyle(fontSize: 15)),
                ),
              ),
              HList(height: 0.07.sh, widget: ServicesList(itemWidth: 0.12.sw)),
              Container(
                decoration: BoxDecoration(
                    borderRadius: BorderRadius.all(Radius.circular(25)),
                    color: Colors.white
                ),
                child: TextButton(
                  onPressed: () async {
                    await Navigator.pushNamed(context, NewAreaView.routeName);
                  },
                  child: const Padding(
                    padding: EdgeInsets.fromLTRB(120.0, 15.0, 120.0, 15.0),
                    child: Text(
                      "New Area",
                      style: TextStyle(
                          fontSize: 20,
                          color: Colors.black,
                          fontWeight: FontWeight.w500),
                    ),
                  ),
                ),
              ),
              Align(
                alignment: Alignment.centerLeft,
                child: const Padding(
                  padding: EdgeInsets.fromLTRB(25, 20, 0, 20),
                  child: Text('Your AREAs', style: TextStyle(fontSize: 15)),
                ),
              ),
              AreaList()
            ],
          ),
      );

  Widget _buildBox({required Color color}) => Container(margin: EdgeInsets.all(12), height: 100, width: 100, color: color);
}
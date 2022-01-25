import 'package:hookhook/widgets/areas_creator.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/widgets/h_list.dart';
import 'package:hookhook/widgets/services_items.dart';
import 'package:hookhook/widgets/your_area_items.dart';

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
          body: Column(
            children: <Widget>[
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: Text('HookHook', textAlign: TextAlign.left,style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45)),
              ),
              const Align(
                alignment: Alignment.centerLeft,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(30, 30, 0, 0),
                  child: Text('Hello, Arthur', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20)),
                ),
              ),
              const Align(
                alignment: Alignment.centerLeft,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(30, 10, 0, 20),
                  child: Text('Welcome back !', style: TextStyle(fontSize: 20, color: Color(0xFF6A6A6A))),
                ),
              ),
              HList(Widgets: ServicesItems.servicesWidgets),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Creator', style: TextStyle(fontSize: 15)),
              ),
              ClipRRect(
                borderRadius: BorderRadius.circular(30.0),
                child: const AreasCreator()
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 13, 0, 4),
                child: Text('Your AREAs', textAlign: TextAlign.left, style: TextStyle(fontSize: 15)),
              ),
              HList(Widgets: YourAreaItems.yourAreaWidgets),
            ],
          ),
      );

  Widget _buildBox({required Color color}) => Container(margin: EdgeInsets.all(12), height: 100, width: 100, color: color);
}
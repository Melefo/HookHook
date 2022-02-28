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
          body: Column(
            children: <Widget>[
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: Text('HookHook', textAlign: TextAlign.left,style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45)),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Services', style: TextStyle(fontSize: 15)),
              ),
              const HList(height: 70, widget: ServicesList(itemWidth: 60)),

              Container(
                decoration: const BoxDecoration(
                    borderRadius: BorderRadius.all(Radius.circular(25)),
                    color: Color(0xFF3B3F43)
                ),
                child: TextButton(
                  onPressed: () {
                    // Respond to button press
                  },
                  child: const Padding(
                    padding: EdgeInsets.fromLTRB(120.0, 15.0, 120.0, 15.0),
                    child: Text(
                      "New Area",
                      style: TextStyle(
                          fontSize: 20,
                          color: Colors.white,
                          fontWeight: FontWeight.w500),
                    ),
                  ),
                ),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 13, 0, 4),
                child: Text('Your AREAs', textAlign: TextAlign.left, style: TextStyle(fontSize: 15)),
              ),
            ],
          ),
      );

  Widget _buildBox({required Color color}) => Container(margin: EdgeInsets.all(12), height: 100, width: 100, color: color);
}
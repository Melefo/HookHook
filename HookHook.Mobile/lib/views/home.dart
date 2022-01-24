import 'dart:async';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/widgets/h_list.dart';

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
                padding: EdgeInsets.only(top: 80),
                child: Text('HookHook', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45)),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 50, 0, 30),
                child: Text('Hello, Arthur', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 30)),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 30),
                child: Text('Welcome back !', style: TextStyle(fontSize: 30)),
              ),
              SizedBox(
                height: 120,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemBuilder: (_, __) => _buildBox(color: Colors.orange),
                ),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Creator', style: TextStyle(fontFamily: 'Comfortaa', fontSize: 15)),
              ),
              ClipRRect(
                borderRadius: BorderRadius.circular(30.0),
                child: Container(
                  color: Color.fromRGBO(59, 63, 67, 100),
                  width: 350,
                  height: 200,
                ),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Your AREAs', style: TextStyle(fontFamily: 'Comfortaa', fontSize: 15)),
              ),
              HList(),
            ],
          ),
      );

  Widget _buildBox({required Color color}) => Container(margin: EdgeInsets.all(12), height: 100, width: 100, color: color);
}
import 'dart:async';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';

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
          body: Container(
            child: Text('test')
          ),
      );
}
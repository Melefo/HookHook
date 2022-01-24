import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class ListItem extends StatelessWidget {

  const ListItem({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.all(12),
      width: 100, //prendre en arg
      height: 100, //prendre en arg
      decoration: BoxDecoration(
          borderRadius: BorderRadius.all(Radius.circular(5)), //prender en arg
          color: Colors.orange //prendre en arg
      ),
      child: Text("Test"), //prendre en arg
    );
  }

}
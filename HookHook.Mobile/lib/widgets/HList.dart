import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'ListItem.dart';

class HList extends StatelessWidget {

  const HList({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
      return Container(
        height: 100,
        child: ListView(
          scrollDirection: Axis.horizontal,
          children: <Widget>[
            ListItem(),
            ListItem(),
            ListItem(),
            ListItem(),
            ListItem(),
          ],
        ),
      );
  }

}
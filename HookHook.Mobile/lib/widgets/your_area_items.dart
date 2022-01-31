import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'list_items.dart';

class YourAreaList extends StatelessWidget {

  final double itemWidth;

  const YourAreaList({Key? key, required this.itemWidth}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ListView(
        physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
        scrollDirection: Axis.horizontal,
        children: <Widget> [
          ListItem(width: itemWidth, cornerRadius: 25, color: const Color(0xFF3B3F43), content: const AreaItem(service: "Twitter", action: "on push", dateTime: "10 décembre 14h23")),
          ListItem(width: itemWidth, cornerRadius: 25, color: const Color(0xFF3B3F43), content: const Icon(Icons.category, color: Colors.orange,)),
          ListItem(width: itemWidth, cornerRadius: 25, color: const Color(0xFF3B3F43), content: const Icon(Icons.category, color: Colors.orange,)),
          ListItem(width: itemWidth, cornerRadius: 25, color: const Color(0xFF3B3F43), content: const Icon(Icons.category, color: Colors.orange,)),
          ListItem(width: itemWidth, cornerRadius: 25, color: const Color(0xFF3B3F43), content: const Icon(Icons.category, color: Colors.orange,))
        ]
    );
  }

}

class AreaItem extends StatelessWidget {

  final String service;
  final String action;
  final String dateTime;

  const AreaItem({Key? key, required this.service, required this.action, required this.dateTime}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(service, style: TextStyle(color: Colors.white)),
            Text(" " + action, style: TextStyle(color: Colors.white)),
          ],
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.category, color: Colors.orange,),
            Icon(Icons.arrow_right_alt_rounded, color: Colors.orange,),
            Icon(Icons.category, color: Colors.orange,),
          ],
        ),
        Text(dateTime, style: TextStyle(color: Colors.white)),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            Container(
              decoration: BoxDecoration(
                  borderRadius: BorderRadius.all(Radius.circular(10)),
                  color: Colors.white
              ),
              child: IconButton(
                icon: const Icon(Icons.refresh),
                onPressed: () {
                  print("Refresh");
                },
              ),
            ),
            Container(
              decoration: BoxDecoration(
                  borderRadius: BorderRadius.all(Radius.circular(10)),
                  color: Colors.white
              ),
              child: IconButton(
                icon: const Icon(Icons.settings),
                onPressed: () {
                  print("Settings");
                },
              ),
            ),
            Container(
              decoration: BoxDecoration(
                  borderRadius: BorderRadius.all(Radius.circular(10)),
                  color: Colors.white
              ),
              child: IconButton(
                icon: const Icon(Icons.delete),
                onPressed: () {
                  print("Trash");
                },
              ),
            ),
          ],
        ),
      ],
    );
  }

}
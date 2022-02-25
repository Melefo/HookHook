import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'list_items.dart';

class ServicesList extends StatelessWidget {

   final double itemWidth;

  const ServicesList({Key? key, required this.itemWidth}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ListView(
        physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
        scrollDirection: Axis.horizontal,
        children: <Widget> [
          ListItem(width: itemWidth, cornerRadius: 15, color: const Color(0xFFA3E7EE), content: const Icon(Icons.category, color: Color(0xFF73B6BD))),
          ListItem(width: itemWidth, cornerRadius: 15, color: const Color(0xFFB4E1DC), content: const Icon(Icons.category, color: Color(0xFF85B1AC))),
          ListItem(width: itemWidth, cornerRadius: 15, color: const Color(0xFFF5CDCB), content: const Icon(Icons.category, color: Color(0xFFC49E9C))),
          ListItem(width: itemWidth, cornerRadius: 15, color: const Color(0xFFF8CBAA), content: const Icon(Icons.category, color: Color(0xFFC79D7D))),
          ListItem(width: itemWidth, cornerRadius: 15, color: const Color(0xFFFFFFC7), content: const Icon(Icons.category, color: Color(0xFFC6C791)))
        ]
    );
  }

}
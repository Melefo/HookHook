import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'list_items.dart';

class ServicesItems {

  static final List<Widget> servicesWidgets = <Widget> [
      const ListItem(cornerRadius: 25, color: Color(0xFFA3E7EE), Content: Icon(Icons.category, color: Color(0xFF73B6BD), size: 70)),
      const ListItem(cornerRadius: 25, color: Color(0xFFB4E1DC), Content: Icon(Icons.category, color: Color(0xFF85B1AC), size: 70)),
      const ListItem(cornerRadius: 25, color: Color(0xFFF5CDCB), Content: Icon(Icons.category, color: Color(0xFFC49E9C), size: 70)),
      const ListItem(cornerRadius: 25, color: Color(0xFFF8CBAA), Content: Icon(Icons.category, color: Color(0xFFC79D7D), size: 70)),
      const ListItem(cornerRadius: 25, color: Color(0xFFFFFFC7), Content: Icon(Icons.category, color: Color(0xFFC6C791), size: 70))
  ];

}
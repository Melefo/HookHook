import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'list_items.dart';

class HList extends StatelessWidget {

  final Widget widget;
  final double height;

  const HList({Key? key, required this.widget, required this.height}) : super(key: key);

  @override
  Widget build(BuildContext context) {
      return SizedBox(
        height: height,
        child: widget,
      );
  }

}
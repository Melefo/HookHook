import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class ListItem extends StatelessWidget {

  final double? cornerRadius;
  final Color? color;
  final Widget content;
  final double width;

  const ListItem({Key? key, required this.width, this.cornerRadius = 0, this.color = Colors.black,required this.content}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(9),
      width: width,
      decoration: BoxDecoration(
          borderRadius: BorderRadius.all(Radius.circular(cornerRadius!)),
          color: color!
      ),
      child: content,
    );
  }

}
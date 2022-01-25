import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class ListItem extends StatelessWidget {

  final double? cornerRadius;
  final Color? color;
  final Widget Content;

  const ListItem({Key? key, this.cornerRadius = 0, this.color = Colors.black,required this.Content}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.all(9),
      width: 120,
      decoration: BoxDecoration(
          borderRadius: BorderRadius.all(Radius.circular(cornerRadius!)),
          color: color!
      ),
      child: Content,
    );
  }

}
import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class HList extends StatelessWidget {

  final List<Widget> Widgets;

  const HList({Key? key, required this.Widgets}) : super(key: key);

  @override
  Widget build(BuildContext context) {
      return SizedBox(
        height: 120,
        child: ListView(
          physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
          scrollDirection: Axis.horizontal,
          children: Widgets,
        ),
      );
  }

}
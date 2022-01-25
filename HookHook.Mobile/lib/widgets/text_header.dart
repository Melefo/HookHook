import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class H1 extends StatelessWidget {

  const H1({Key? key}): super(key: key);

  @override
  Widget build(BuildContext context) {
    return Text('hello', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45));
  }

}
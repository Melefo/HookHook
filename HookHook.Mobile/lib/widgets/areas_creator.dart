import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class AreasCreator extends StatelessWidget {

  const AreasCreator({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
        color: const Color(0xFF3B3F43),
        width: 350,
        height: 300,
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Column(
            children: [
              Text('Your Area name :', style: TextStyle(color: Colors.white)),
              TextField(),
            ],
          ),
        )
    );
  }
}
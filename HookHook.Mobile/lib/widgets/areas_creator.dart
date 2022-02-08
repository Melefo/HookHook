import 'package:flutter/material.dart';
import 'package:hookhook/widgets/reaction_dropdown.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

import 'action_dropdown.dart';

class AreasCreator extends StatefulWidget {
  const AreasCreator({Key? key}) : super(key: key);

  @override
  _AreasCreator createState() => _AreasCreator();
}

class _AreasCreator extends State<AreasCreator> {

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
              Row(
                children: [
                  Text('When : ', style: TextStyle(color: Colors.white)),
                  ActionDropdown(),
                ],
              ),
              Row(
                children: [
                  Text('Do : ', style: TextStyle(color: Colors.white)),
                  ReactionDropdown(),
                ],
              ),
              ElevatedButton.icon(
                onPressed: () {
                  // Respond to button press
                },
                icon: Icon(Icons.add, size: 18, color: Colors.white),
                label: Text("ADD"),
              )
            ],
          ),

        )
    );
  }
}
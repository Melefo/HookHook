import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class AreasCreator extends StatefulWidget {
  @override
  _AreasCreator createState() => _AreasCreator();
}

class _AreasCreator extends State<AreasCreator> {

  String dropdownvalue = 'Tweet';

  var items =  ['Tweet', 'Push', 'Like', 'Message Pinned'];

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
                  DropdownButton<String>(
                    items: <String>['Tweet', 'Push', 'Like', 'Message Pinned'].map((String value) {
                      return DropdownMenuItem<String>(
                        value: value,
                        child: Text(value),
                      );
                    }).toList(),
                    onChanged: (_) {},
                  ),
                ],
              ),
              Row(
                children: [
                  Text('Do : ', style: TextStyle(color: Colors.white)),
                  DropdownButton(
                    value: dropdownvalue,
                    icon: Icon(Icons.keyboard_arrow_down),
                    items:items.map((String items) {
                      return DropdownMenuItem(
                          value: items,
                          child: Text(items)
                      );
                    }
                    ).toList(),
                    onChanged: (String newValue){
                      setState(() {
                        dropdownvalue = newValue;
                      });
                    },
                  ),
                ],
              ),
              ElevatedButton.icon(
                onPressed: () {
                  // Respond to button press
                },
                icon: Icon(Icons.add, size: 18),
                label: Text("CONTAINED BUTTON"),
              )
            ],
          ),

        )
    );
  }
}
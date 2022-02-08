import 'package:flutter/material.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class ActionDropdown extends StatefulWidget {
  const ActionDropdown({Key? key}) : super(key: key);

  @override
  _ActionDropdown createState() => _ActionDropdown();
}

class _ActionDropdown extends State<ActionDropdown> {

  String _dropdownvalue = 'Tweet';

  var items =  ['Tweet', 'Push', 'Like', 'Message Pinned'];

  @override
  Widget build(BuildContext context) {
    return DropdownButton<String>(
          focusColor:Colors.white,
          value: _dropdownvalue,
          //elevation: 5,
          style: TextStyle(color: Colors.white),
          iconEnabledColor:Colors.white,
          items: items.map<DropdownMenuItem<String>>((String value) {
            return DropdownMenuItem<String>(
              value: value,
              child: Text(value,style:TextStyle(color:Colors.white),),
            );
          }).toList(),
          hint:Text(
            "Please choose a langauage",
            style: TextStyle(
                color: Colors.white,
                fontSize: 14,
                fontWeight: FontWeight.w500),
          ),
          onChanged: (String? value) {
            setState(() {
              if (value != null) {
                _dropdownvalue = value;
              }
            });
          },
    );
  }
}
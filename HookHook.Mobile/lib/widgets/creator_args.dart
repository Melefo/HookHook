import 'package:flutter/material.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/service_info_model.dart';
import '../adaptive_state.dart';

class CreatorArgs extends StatefulWidget {
  CreatorArgs({Key? key, required this.action, required this.areaType, required this.onUpdate})
      : parameters = action.events!
      .singleWhere((element) =>
  element.className == action.choice.action &&
      element.areaType == areaType)
      .parameterNames,
        controllers = action.events!
            .singleWhere((element) =>
        element.className == action.choice.action &&
            element.areaType == areaType)
            .parameterNames.map((e) => TextEditingController()).toList(),
        super(key: key);

  final ActionParameters action;
  final AreaType areaType;

  final List<String> parameters;
  final List<TextEditingController> controllers;

  final Function(String) onUpdate;

  @override
  _CreatorArgs createState() => _CreatorArgs();
}

class _CreatorArgs extends AdaptiveState<CreatorArgs> {

  @override
  Widget build(BuildContext context) {
    return Column(
        children: widget.parameters
            .asMap()
            .entries
            .map((e) =>
            TextFormField(
              controller: widget.controllers[e.key],
              decoration: InputDecoration(
                enabledBorder: UnderlineInputBorder(
                    borderSide: BorderSide(
                        color: darkMode ? Colors.white : Colors
                            .black
                    )
                ),
                labelText: e.value,
                labelStyle: TextStyle(
                    color: darkMode ? Colors.white : Colors.black
                ),
              ),
              style: TextStyle(
                  color: darkMode ? Colors.white : Colors.black
              ),
              onChanged: (text) {
                if (widget.action.choice.args == null) {
                  widget.action.choice.args = [];
                  for (var _ in widget.controllers) {
                    widget.action.choice.args!.add("");
                  }
                }
                widget.action.choice.args![e.key] = text;
                widget.onUpdate(text);
              },
            )).toList()
    );
  }
}
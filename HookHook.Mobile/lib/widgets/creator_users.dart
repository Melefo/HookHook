import 'package:flutter/material.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/wrapper/service.dart';
import '../adaptive_state.dart';

class CreatorUsers extends StatefulWidget {
  const CreatorUsers({Key? key, required this.action, required this.onUpdate}) : super(key: key);

  final ActionParameters action;
  final Function(String userId) onUpdate;

  @override
  _CreatorUsers createState() => _CreatorUsers();
}

class _CreatorUsers extends AdaptiveState<CreatorUsers> {

  @override
  Widget build(BuildContext context) =>
      DropdownButton(
        value: widget.action.choice.userId,
        hint: const Text("Choose your Account"),
        items: <DropdownMenuItem>[
          for (Account element in widget.action.accounts!) DropdownMenuItem(
            value: element.userId,
            child: Text(element.username),
          )
        ],
        onChanged: (dynamic value) async {
          setState(() {
            widget.action.choice.userId = value!;
            if (value != null) {
              widget.onUpdate(value!);
            }
          });
        },
      );
}
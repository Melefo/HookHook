import 'package:flutter/material.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:hookhook/wrapper/service.dart';
import '../adaptive_state.dart';

class CreatorUsers extends StatefulWidget {
  const CreatorUsers({Key? key, required this.acc, required this.onUpdate}) : super(key: key);

  final List<Account> acc;
  final Function(String userId) onUpdate;

  @override
  _CreatorUsers createState() => _CreatorUsers();
}

class _CreatorUsers extends AdaptiveState<CreatorUsers> {
  String ?dd_value;

  @override
  Widget build(BuildContext context) =>
      DropdownButton(
        value: dd_value,
        hint: const Text("Choose your Account"),
        items: <DropdownMenuItem>[
          for (Account element in widget.acc) DropdownMenuItem(
            value: element.userId,
            child: Text(element.username),
          )
        ],
        onChanged: (dynamic value) async {
          dd_value = value!.toString();
          setState(() {
            if (value != null) {
              widget.onUpdate(dd_value!);
            }
          });
        },
      );
}
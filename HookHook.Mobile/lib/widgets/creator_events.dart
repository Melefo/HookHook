import 'package:flutter/material.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/service_info_model.dart';
import '../adaptive_state.dart';

class CreatorEvents extends StatefulWidget {
  const CreatorEvents({Key? key, required this.action, required this.onUpdate, required this.areaType}) : super(key: key);

  final ActionParameters action;
  final AreaType areaType;
  final Function(String className) onUpdate;

  @override
  _CreatorEvents createState() => _CreatorEvents();
}

class _CreatorEvents extends AdaptiveState<CreatorEvents> {
  late List<ServicesInfoModel> possibleAction;

  @override
  void initState() {
    possibleAction = widget.action.events!.where(
            (element) =>element.name.toLowerCase() == widget.action.choice.service!.toLowerCase() && element.areaType == widget.areaType
    ).toList();
    super.initState();
  }

  @override
  Widget build(BuildContext context) =>
      DropdownButton(
        value: widget.action.choice.action,
        hint: const Text("Choose your Event"),
        items: <DropdownMenuItem>[
          for (ServicesInfoModel element in possibleAction) DropdownMenuItem(
            value: element.className,
            child: Text(element.description)
          )
        ],
        onChanged: (dynamic value) async {
          setState(() {
            widget.action.choice.action = value!;
            if (value != null) {
              widget.onUpdate(value!);
            }
          });
        },
      );
}
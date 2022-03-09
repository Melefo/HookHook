import 'package:flutter/material.dart';
import 'package:hookhook/models/service_info_model.dart';
import '../adaptive_state.dart';

class CreatorEvents extends StatefulWidget {
  const CreatorEvents({Key? key, required this.events, required this.onUpdate, required this.areaType, required this.chosenService}) : super(key: key);

  final String chosenService;
  final List<ServicesInfoModel> events;
  final AreaType areaType;
  final Function(String className) onUpdate;

  @override
  _CreatorEvents createState() => _CreatorEvents();
}

class _CreatorEvents extends AdaptiveState<CreatorEvents> {
  late List<ServicesInfoModel> possibleAction;
  String ?dd_value;

  @override
  void initState() {
    possibleAction = widget.events.where((element) => element.name.toLowerCase() == widget.chosenService.toLowerCase() && element.areaType == widget.areaType).toList();
    super.initState();
  }

  @override
  Widget build(BuildContext context) =>
      DropdownButton(
        value: dd_value,
        hint: const Text("Choose your Event"),
        items: <DropdownMenuItem>[
          for (ServicesInfoModel element in possibleAction) DropdownMenuItem(
            value: element.className,
            child: Text(element.description)
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
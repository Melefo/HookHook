import 'package:flutter/material.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/widgets/services_icon.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:hookhook/wrapper/service.dart';
import '../adaptive_state.dart';
import 'package:hookhook/wrapper/about.dart' as about;

class CreatorServices extends StatefulWidget {
  const CreatorServices({Key? key, required this.services, required this.action, required this.onUpdate}) : super(key: key);

  final List<about.Service> services;
  final ActionParameters action;
  final Function(List<Account>, List<ServicesInfoModel>, String) onUpdate;

  @override
  _CreatorServices createState() => _CreatorServices();
}

class _CreatorServices extends AdaptiveState<CreatorServices> {
  @override
  Widget build(BuildContext context) =>
      DropdownButton(
          value: widget.action.choice.service,
          hint: const Text("Choose your Service"),
          items: <DropdownMenuItem>[
            for (about.Service element in widget.services) DropdownMenuItem(
              value: element.name,
              child: Row(
                children: [
                  ServiceIcon.serviceIcon(element.name),
                  Text(element.name),
                ],
              ),
            )
          ],
          onChanged: (dynamic value) async {
            setState(() {
              widget.action.choice.service = value!;
            });
            if (value != null) {
              final accounts = await Backend().service.getAccounts(value!);
              final actions = await Backend().area.getServices();
              setState(() {
                  widget.action.accounts = accounts;
                  widget.action.events = actions;
                  widget.onUpdate(accounts, actions, value!);
              });
            }
          },
        );
}
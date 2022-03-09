import 'package:flutter/material.dart';
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/widgets/services_icon.dart';
import 'package:hookhook/wrapper/backend.dart';
import 'package:hookhook/wrapper/service.dart';
import '../adaptive_state.dart';
import 'package:hookhook/wrapper/about.dart' as about;

class CreatorServices extends StatefulWidget {
  const CreatorServices({Key? key, required this.services, required this.onUpdate}) : super(key: key);

  final List<about.Service> services;
  final Function(List<Account>, List<ServicesInfoModel>, String) onUpdate;

  @override
  _CreatorServices createState() => _CreatorServices();
}

class _CreatorServices extends AdaptiveState<CreatorServices> {
  String ?dd_value;
  List<Account> ?acc;
  List<ServicesInfoModel> ?serv;

  @override
  Widget build(BuildContext context) =>
      DropdownButton(
          value: dd_value,
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
            dd_value = value!.toString();
            final accounts = await Backend().service.getAccounts(dd_value!);
            final actions = await Backend().area.getServices();
            setState(() {
              if (value != null) {
                acc = accounts;
                serv = actions;
                widget.onUpdate(acc!, serv!, dd_value!);
              }
            });
          },
        );
}
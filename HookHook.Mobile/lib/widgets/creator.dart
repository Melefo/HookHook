import 'package:flutter/cupertino.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/wrapper/about.dart' as about;

import '../adaptive_state.dart';
import 'creator_args.dart';
import 'creator_events.dart';
import 'creator_services.dart';
import 'creator_users.dart';

class Creator extends StatefulWidget {
  const Creator({Key? key, required this.areaType, required this.services, required this.action, required this.onUpdate}) : super(key: key);

  final ActionParameters action;
  final AreaType areaType;
  final List<about.Service> services;

  final Function onUpdate;

  @override
  _Creator createState() => _Creator();
}

class _Creator extends AdaptiveState<Creator> {
  @override
  Widget build(BuildContext context) =>
      Column(
        children: [
          CreatorServices(
              services: widget.services,
              action: widget.action,
              onUpdate: (accounts, servicesInfo,
                  chosenService) {
                setState(() {
                  widget.action.choice.userId = null;
                  widget.action.choice.args = null;
                  widget.action.choice.action = null;
                });
              }
          ),
          if (widget.action.choice.service != null &&
              widget.action.accounts != null) CreatorUsers(
              action: widget.action,
              onUpdate: (user) {
                setState(() {
                  widget.action.choice.args = null;
                  widget.action.choice.action = null;
                });
              }
          ),
          if (widget.action.events != null &&
              widget.action.choice.service != null &&
              widget.action.choice.userId != null) CreatorEvents(
            action: widget.action,
            areaType: AreaType.ACTION,
            onUpdate: (chosenAction) {
              setState(() {
                widget.action.choice.args = null;
              });
            },
          ),
          if (widget.action.choice.action != null &&
              widget.action.events != null)
            CreatorArgs(
                action: widget.action,
                areaType: AreaType.ACTION,
                onUpdate: (String text) {
                  widget.onUpdate();
                },
            )
        ],
      );
}
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/models/action_parameters.dart';
import 'package:hookhook/models/service_info_model.dart';
import 'package:hookhook/wrapper/about.dart' as about;

import '../adaptive_state.dart';
import '../hookhook_colors.dart';
import 'creator_args.dart';
import 'creator_events.dart';
import 'creator_services.dart';
import 'creator_users.dart';

class Creator extends StatefulWidget {
  const Creator({Key? key, required this.areaType, required this.services, required this.action, required this.onUpdate, this.onDelete}) : super(key: key);

  final ActionParameters action;
  final AreaType areaType;
  final List<about.Service> services;

  final Function onUpdate;
  final Function? onDelete;

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
            areaType: widget.areaType,
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
                areaType: widget.areaType,
                onUpdate: (String text) {
                  widget.onUpdate();
                },
            ),
          if (widget.action.events != null && widget.action.choice.action != null && widget.action.events!.singleWhere((element) => element.areaType == widget.areaType && element.className == widget.action.choice.action).formatters != null)
            Text(
                "You may use the following formatters to get data from your WHEN in your DO: ${widget.action.events!.singleWhere((element) => element.areaType == widget.areaType && element.className == widget.action.choice.action).formatters!.map((e) => '{$e}').join(', ')}",
              style: const TextStyle(
                color: Colors.grey
              ),
            ),
          if (widget.onDelete != null)
            TextButton(
              onPressed: () => widget.onDelete!(),
              style: TextButton.styleFrom(
                  backgroundColor: darkMode
                      ? HookHookColors.gray
                      : Colors.white,
                  padding: const EdgeInsets.all(12),
                  minimumSize: Size.zero,
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10)
                  )
              ),
              clipBehavior: Clip.none,
              child: Icon(
                  Icons.delete,
                  color: darkMode
                      ? Colors.white
                      : HookHookColors.gray
              ),
            )
        ],
      );
}
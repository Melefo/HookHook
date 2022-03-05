import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Twitch extends StatefulWidget {
  final double width;
  final bool enabled;
  const Twitch({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Twitch createState() => _Twitch();
}

class _Twitch extends AdaptiveState<Twitch> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFFFFFC7),
        title: const Center(child: Text("Twitch")),
        children: [

        ],
      );

  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFFFFFC7),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.twitch(
                widget.width, const Color(0xFFC6C791)),
          ),
          onPressed: widget.enabled ? () {
            showDialog<String>(
                context: context,
                builder: buildDialog
            );
          } : null,
        )
    );
  }
}
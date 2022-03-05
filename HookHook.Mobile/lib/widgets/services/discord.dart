import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Discord extends StatefulWidget {
  final double width;
  final bool enabled;
  const Discord({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Discord createState() => _Discord();
}

class _Discord extends AdaptiveState<Discord> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFD9D1EA),
        title: const Center(child: Text("Discord")),
        children: [

        ],
      );

  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFD9D1EA),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.discord(
                widget.width, const Color(0xFFAAA3BA)),
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
import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Google extends StatefulWidget {
  final double width;
  final bool enabled;
  const Google({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Google createState() => _Google();
}

class _Google extends AdaptiveState<Google> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFF8CBAA),
        title: const Center(child: Text("Google")),
        children: [

        ],
      );

  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFF8CBAA),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.google(
                widget.width, const Color(0xFFC79D7D)),
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
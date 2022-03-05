import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Twitter extends StatefulWidget {
  final double width;
  final bool enabled;
  const Twitter({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Twitter createState() => _Twitter();
}

class _Twitter extends AdaptiveState<Twitter> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFA3E7EE),
        title: const Center(child: Text("Twitter")),
        children: [

        ],
      );

  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFA3E7EE),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.twitter(
                widget.width, const Color(0xFF73B6BD)),
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
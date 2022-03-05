import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Github extends StatefulWidget {
  final double width;
  final bool enabled;
  const Github({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Github createState() => _Github();
}

class _Github extends AdaptiveState<Github> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFF5CDCB),
        title: const Center(child: Text("GitHub")),
        children: [

        ],
      );

  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFF5CDCB),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.gitHub(
                widget.width, const Color(0xFFC49E9C)),
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
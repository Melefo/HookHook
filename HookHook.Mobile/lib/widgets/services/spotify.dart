import 'package:flutter/material.dart';
import 'package:hookhook/adaptive_state.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Spotify extends StatefulWidget {
  final double width;
  final bool enabled;
  const Spotify({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Spotify createState() => _Spotify();
}

class _Spotify extends AdaptiveState<Spotify> {
  Widget buildDialog(BuildContext context) =>
      SimpleDialog(
        backgroundColor: const Color(0xFFB4E1DC),
        title: const Center(child: Text("Spotify")),
        children: [

        ],
    );

  @override
  Widget build(BuildContext context) =>
      ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFB4E1DC),
        content: IconButton(
          icon: Padding(
            padding: const EdgeInsets.all(4),
            child: ServicesIcons.spotify(
                widget.width, const Color(0xFF85B1AC)),
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
import 'package:flutter/material.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/widgets/services/discord.dart';
import 'package:hookhook/widgets/services/github.dart';
import 'package:hookhook/widgets/services/google.dart';
import 'package:hookhook/widgets/services/spotify.dart';
import 'package:hookhook/widgets/services/twitch.dart';
import 'package:hookhook/widgets/services/twitter.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import '../main.dart';
import 'list_items.dart';

class ServicesList extends StatelessWidget {

  final double itemWidth;

  const ServicesList({Key? key, required this.itemWidth}) : super(key: key);

  List<Widget> generateFromServices() {
    List<Widget> list = [];
    if (HookHook.backend.about == null) {
      return [];
    }

    for (var service in HookHook.backend.about!.server.services) {
      switch (service.name.toLowerCase()) {
        case "spotify":
          {
            list.add(Spotify(width: itemWidth, enabled: true));
            break;
          }
        case "discord":
          {
            list.add(Discord(width: itemWidth, enabled: true));
            break;
          }
        case "github":
          {
            list.add(Github(width: itemWidth, enabled: true));
            break;
          }
        case "google":
        case "youtube":
          {
            list.add(Google(width: itemWidth, enabled: true));
            break;
          }
        case "twitch":
          {
            list.add(Twitch(width: itemWidth, enabled: true));
            break;
          }
        case "twitter":
          {
            list.add(Twitter(width: itemWidth, enabled: true));
            break;
          }
      }
    }
    return list;
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
        physics: const BouncingScrollPhysics(
            parent: AlwaysScrollableScrollPhysics()),
        scrollDirection: Axis.horizontal,
        children: generateFromServices()
    );
  }
}
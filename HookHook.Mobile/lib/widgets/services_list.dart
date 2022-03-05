import 'package:flutter/material.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/wrapper/backend.dart';
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
        case "spotify": {
          list.add(ListItem(
              width: itemWidth,
              cornerRadius: 15,
              color: const Color(0xFFB4E1DC),
              content: Padding(
                padding: EdgeInsets.all(itemWidth / 6),
                child: ServicesIcons.spotify(itemWidth, const Color(0xFF85B1AC)),
              ),
          ));
          break;
        }
        case "discord": {
          list.add(ListItem(
            width: itemWidth,
            cornerRadius: 15,
            color: const Color(0xFFD9D1EA),
            content: Padding(
              padding: EdgeInsets.all(itemWidth / 6),
              child: ServicesIcons.discord(itemWidth, const Color(0xFFAAA3BA)),
            ),
          ));
          break;
        }
        case "github": {
          list.add(ListItem(
              width: itemWidth,
              cornerRadius: 15,
              color: const Color(0xFFF5CDCB),
              content: Padding(
                padding: EdgeInsets.all(itemWidth / 6),
                child: ServicesIcons.gitHub(itemWidth, const Color(0xFFC49E9C)),
              ),
          ),);
          break;
        }
        case "google":
        case "youtube": {
          list.add(ListItem(
              width: itemWidth,
              cornerRadius: 15,
              color: const Color(0xFFF8CBAA),
              content: Padding(
                padding: EdgeInsets.all(itemWidth / 6),
                child: ServicesIcons.google(itemWidth, const Color(0xFFC79D7D)),
              )
          ));
          break;
        }
        case "twitch": {
          list.add(ListItem(
              width: itemWidth,
              cornerRadius: 15,
              color: const Color(0xFFFFFFC7),
              content: Padding(
                padding: EdgeInsets.all(itemWidth / 6),
                child: ServicesIcons.twitch(itemWidth, const Color(0xFFC6C791)),
              )
          ));
          break;
        }
        case "twitter": {
          list.add(ListItem(
              width: itemWidth,
              cornerRadius: 15,
              color: const Color(0xFFA3E7EE),
              content: Padding(
                padding: EdgeInsets.all(itemWidth / 6),
                child: ServicesIcons.twitter(itemWidth, const Color(0xFF73B6BD)),
              )
          ),);
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
import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/widgets/services/discord.dart';
import 'package:hookhook/widgets/services/github.dart';
import 'package:hookhook/widgets/services/google.dart';
import 'package:hookhook/widgets/services/spotify.dart';
import 'package:hookhook/widgets/services/twitch.dart';
import 'package:hookhook/widgets/services/twitter.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class AreaItem extends StatefulWidget {
  final String areaName;
  final String datetime;
  final String from;
  final String areaId;
  final List<String> to;
  final Function delete;
  final Function trigger;
  final bool failed;

  const AreaItem(
      {Key? key, this.areaName = "Area Name", required this.areaId, this.datetime = "dateTime", this.from = "Action", this.to = const ["Reactions"], required this.delete, required this.trigger, required this.failed})
      : super(key: key);
  
  @override
  _AreaItem createState() => _AreaItem();
}

class _AreaItem extends AdaptiveState<AreaItem> {
  Widget serviceIcon(String provider) {
    switch (provider.toLowerCase()) {
      case "discord":
        {
          return const Discord(width: 50, padding: 1);
        }
      case "github":
        {
          return const Github(width: 50, padding: 1);
        }
      case "youtube":
      case "google":
        {
          return const Google(width: 50, padding: 1);
        }
      case "spotify":
        {
          return const Spotify(width: 50, padding: 1);
        }
      case "twitch":
        {
          return const Twitch(width: 50, padding: 1);
        }
      case "twitter":
        {
          return const Twitter(width: 50, padding: 1);
        }
    }
    return const Icon(Icons.category, size: 20);
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: Container(
        alignment: Alignment.topCenter,
        padding: const EdgeInsets.all(10.0),
        width: 350,
        decoration: BoxDecoration(
          borderRadius: const BorderRadius.all(Radius.circular(25)),
          color: darkMode ? HookHookColors.gray : Colors.white,
        ),
        child: Column(
          children: [
            Text(
                widget.areaName,
                style: TextStyle(
                    color: darkMode ? Colors.white : Colors.black,
                  fontSize: 12.sp
                )
            ),
            Padding(
              padding: const EdgeInsets.only(top: 10),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  serviceIcon(widget.from),
                  Icon(
                      Icons.arrow_right_alt_rounded,
                      color: darkMode ? Colors.white : Colors.black
                  ),
                  for (String elem in widget.to) serviceIcon(elem),
                ],
              ),
            ),
            Padding(
              padding: const EdgeInsets.only(top: 10, right: 5, left: 5),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  Text(widget.datetime,
                      style: TextStyle(
                          color: darkMode ? Colors.white : Colors.black
                      )
                  ),
                ],
              ),
            ),
            Padding(
              padding: const EdgeInsets.only(top: 6),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  if (widget.failed)
                    Icon(
                        Icons.warning,
                        color: darkMode ? Colors.white : Colors.black
                    ),
                  Container(
                    decoration: BoxDecoration(
                      borderRadius: const BorderRadius.all(Radius.circular(10)),
                      color: darkMode ? HookHookColors.dark : HookHookColors
                          .light,
                    ),
                    child: IconButton(
                      onPressed: () => widget.trigger(),
                      icon: Icon(
                          Icons.refresh,
                          color: darkMode ? Colors.white : HookHookColors.gray
                      ),
                    ),
                  ),
                  Container(
                    decoration: BoxDecoration(
                      borderRadius: const BorderRadius.all(Radius.circular(10)),
                      color: darkMode ? HookHookColors.dark : HookHookColors
                          .light,
                    ),
                    child: IconButton(
                        onPressed: () {
                          showDialog<String>(
                              context: context,
                              builder: (BuildContext context) =>
                                  AlertDialog(
                                    backgroundColor: darkMode ? HookHookColors
                                        .gray : HookHookColors.light,
                                    content: const Text(
                                        'You will delete this area'
                                    ),
                                    actions: <Widget>[
                                      TextButton(
                                        onPressed: () => Navigator.pop(context, 'Cancel'),
                                        child: const Text(
                                            'Cancel',
                                            style: TextStyle(
                                                color: HookHookColors.orange
                                            )
                                        ),
                                      ),
                                      TextButton(
                                        onPressed: () {
                                          widget.delete();
                                          Navigator.pop(context, 'OK');
                                        },
                                        child: const Text
                                          (
                                            'Yes',
                                            style: TextStyle(
                                                color: HookHookColors.blue
                                            )
                                        ),
                                      ),
                                    ],
                                  )
                          );
                        },
                        icon: Icon(
                            Icons.delete,
                            color: darkMode ? Colors.white : HookHookColors.gray
                        )
                    ),
                  )
                ],
              ),
            ),

          ],
        ),
      ),
    );
  }
}
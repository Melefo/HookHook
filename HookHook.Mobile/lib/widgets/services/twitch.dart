import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_twitch_auth/flutter_twitch_auth.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/wrapper/service.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Twitch extends StatefulWidget {
  final double width;
  final double padding;
  final bool enabled;
  const Twitch({Key? key, required this.width, this.padding = 4, this.enabled = false}) : super(key: key);

  @override
  _Twitch createState() => _Twitch();
}

class _Twitch extends AdaptiveState<Twitch> {
  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFFFFFC7),
        content: IconButton(
          icon: Padding(
            padding: EdgeInsets.all(widget.padding),
            child: ServicesIcons.twitch(
                widget.width, const Color(0xFFC6C791)),
          ),
          onPressed: widget.enabled ? () {
            showDialog<String>(
                context: context,
                builder: (context) => const TwitchDialog()
            );
          } : null,
        )
    );
  }
}

class TwitchDialog extends StatefulWidget {
  const TwitchDialog({Key? key}) : super(key: key);

  @override
  _TwitchDialog createState() => _TwitchDialog();
}

class _TwitchDialog extends AdaptiveState<TwitchDialog> {
  List<Account> accounts = [];

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Twitch").then((value) =>
        setState(() {
          accounts = value;
        })
    );
  }

  @override
  Widget build(BuildContext context) =>
      SimpleDialog(
          backgroundColor: const Color(0xFFFFFFC7),
          title: const Center(
              child: Text(
                  "Twitch",
                  style: TextStyle(
                      color: Colors.black
                  )
              )
          ),
          children: [
            ...List<Widget>.from(
                accounts.map((e) =>
                    Row(
                        mainAxisAlignment: MainAxisAlignment
                            .spaceAround,
                        children: [
                          Text(
                              e.username,
                              style: const TextStyle(
                                  color: Colors.black
                              )
                          ),
                          IconButton(
                              onPressed: () async {
                                await HookHook.backend.service
                                    .deleteAccount(
                                    "Twitch", e.userId);
                                setState(() {
                                  accounts.remove(e);
                                });
                              },
                              icon: const Icon(
                                  Icons.close,
                                  color: Colors.black
                              )
                          )
                        ]
                    ),
                )
            ),
            Padding(
              padding: const EdgeInsets.symmetric(
                  horizontal: 50,
                  vertical: 8
              ),
              child: TextButton(
                  onPressed: () async {
                    final scopes = [
                      "channel:read:subscriptions",
                      "channel:manage:broadcast",
                      "user:read:broadcast",
                      "user:read:subscriptions",
                      "user:edit",
                      "user:read:email",
                      "user:read:follows"
                    ];
                    FlutterTwitchAuth.initialize(
                        twitchClientId: dotenv.env["TWITCH_CLIENTID"]!,
                        twitchRedirectUri: dotenv.env["TWITCH_REDIRECT"]!,
                        twitchClientSecret: '',
                        scope: scopes.join(' ')
                    );
                    String? code = await FlutterTwitchAuth.authToCode(context);
                    final account = await HookHook.backend.service.addTwitch(
                        code!);
                    if (account != null) {
                      setState(() {
                        accounts.add(account);
                      });
                    }
                  },
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: const [
                      Icon(
                          Icons.add,
                          color: Colors.black
                      ),
                      Padding(
                          padding: EdgeInsets.symmetric(
                              horizontal: 4
                          )
                      ),
                      Text(
                          "ADD ACCOUNT",
                          style: TextStyle(color: Colors.black)
                      ),
                    ],
                  ),
                  style: TextButton.styleFrom(
                      backgroundColor: HookHookColors.light,
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10)
                      )
                  )
              ),
            )
          ]
      );

}
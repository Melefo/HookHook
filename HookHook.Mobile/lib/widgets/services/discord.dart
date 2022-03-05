import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/wrapper/service.dart';
import 'package:pkce/pkce.dart';
import 'package:uni_links/uni_links.dart';
import 'package:url_launcher/url_launcher.dart';

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
                builder: (context) => const DiscordDialog()
            );
          } : null,
        )
    );
  }
}

class DiscordDialog extends StatefulWidget {
  const DiscordDialog({Key? key}) : super(key: key);

  @override
  _DiscordDialog createState() => _DiscordDialog();
}

class _DiscordDialog extends AdaptiveState<DiscordDialog> {
  late StreamSubscription listener;
  List<Account> accounts = [];
  final pkcePair = PkcePair.generate();

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Discord").then((value) =>
        setState(() {
          accounts = value;
        })
    );
    listener = linkStream.listen((String? response) async {
      if (response!.startsWith(dotenv.env["DISCORD_REDIRECT"]!)) {
        final url = Uri.parse(response);
        final account = await HookHook.backend.service.addDiscord(url.queryParameters["code"]!, pkcePair.codeVerifier);
        if (account != null) {
          setState(() {
            accounts.add(account);
          });
        }
      }
    });
  }

  @override
  void dispose() {
    listener.cancel();
    super.dispose();
  }

  Future<void> redirect(String authUri) async {
    if (await canLaunch(authUri)) {
      await launch(authUri);
    }
  }

  @override
  Widget build(BuildContext context) =>
      SimpleDialog(
          backgroundColor: const Color(0xFFD9D1EA),
          title: const Center(
              child: Text(
                  "Discord",
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
                                    "Discord", e.userId);
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
                      "identify",
                      "guilds",
                      "email",
                      "bot"
                    ];
                    await redirect("https://discord.com/oauth2/authorize?code_challenge=${pkcePair.codeChallenge}&code_challenge_method=S256&client_id=${dotenv.env["DISCORD_CLIENTID"]}&redirect_uri=${dotenv.env["DISCORD_REDIRECT"]}&response_type=code&scope=${scopes.join(' ')}&permissions=66568");
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
                  style: ButtonStyle(
                    backgroundColor: MaterialStateProperty.all(
                        HookHookColors.light),
                    shape: MaterialStateProperty.all(
                        RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(10)
                        )
                    ),
                  )
              ),
            )
          ]
      );

}
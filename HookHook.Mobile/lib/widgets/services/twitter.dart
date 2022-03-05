import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/wrapper/service.dart';
import 'package:uni_links/uni_links.dart';
import 'package:url_launcher/url_launcher.dart';

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
                builder: (context) => const TwitterDialog()
            );
          } : null,
        )
    );
  }
}

class TwitterDialog extends StatefulWidget {
  const TwitterDialog({Key? key}) : super(key: key);

  @override
  _TwitterDialog createState() => _TwitterDialog();
}

class _TwitterDialog extends AdaptiveState<TwitterDialog> {
  late StreamSubscription listener;
  List<Account> accounts = [];

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Twitter").then((value) =>
        setState(() {
          accounts = value;
        })
    );
    listener = linkStream.listen((String? response) async {
      if (response!.startsWith(dotenv.env["TWITTER_REDIRECT"]!)) {
        final url = Uri.parse(response);
        String code = url.queryParameters["oauth_token"]!;
        String verifier = url.queryParameters["oauth_verifier"]!;
        final account = await HookHook.backend.service.addTwitter(
            code, verifier);
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
          backgroundColor: const Color(0xFFA3E7EE),
          title: const Center(child: Text("Twitter")),
          children: [
            ...List<Widget>.from(
                accounts.map((e) =>
                    Row(
                        mainAxisAlignment: MainAxisAlignment
                            .spaceAround,
                        children: [
                          Text(e.username),
                          IconButton(
                              onPressed: () async {
                                await HookHook.backend.service
                                    .deleteAccount(
                                    "Twitter", e.userId);
                                setState(() {
                                  accounts.remove(e);
                                });
                              },
                              icon: const Icon(Icons.close)
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
                    String? url = await HookHook.backend.signIn
                        .authorize(
                        "Twitter", dotenv.env["TWITTER_REDIRECT"]!);
                    await redirect(url!);
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
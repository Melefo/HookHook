import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:hookhook/main.dart';
import 'package:hookhook/wrapper/service.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../services_icons.dart';
import '../list_items.dart';

class Google extends StatefulWidget {
  final double width;
  final double padding;
  final bool enabled;
  const Google({Key? key, required this.width, this.padding = 4, this.enabled = false}) : super(key: key);

  @override
  _Google createState() => _Google();
}

class _Google extends AdaptiveState<Google> {
  @override
  Widget build(BuildContext context) {
    return ListItem(
        width: widget.width,
        cornerRadius: 15,
        color: const Color(0xFFF8CBAA),
        content: IconButton(
          icon: Padding(
            padding: EdgeInsets.all(widget.padding),
            child: ServicesIcons.google(
                widget.width, const Color(0xFFC79D7D)),
          ),
          onPressed: widget.enabled ? () {
            showDialog<String>(
                context: context,
                builder: (context) => const GoogleDialog()
            );
          } : null,
        )
    );
  }
}

class GoogleDialog extends StatefulWidget {
  const GoogleDialog({Key? key}) : super(key: key);

  @override
  _GoogleDialog createState() => _GoogleDialog();
}

class _GoogleDialog extends AdaptiveState<GoogleDialog> {
  late StreamSubscription listener;
  List<Account> accounts = [];
  GoogleSignIn google = GoogleSignIn(
    // Optional clientId
    clientId: const String.fromEnvironment('GOOGLE_CLIENTID'),
    scopes: [
      "openid",
      "email",
      "profile",
      "https://www.googleapis.com/auth/youtube",
      "https://www.googleapis.com/auth/youtube.readonly",
      "https://www.googleapis.com/auth/youtube.force-ssl"
    ],
  );

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Google").then((value) =>
        setState(() {
          accounts = value;
        })
    );
    listener = google.onCurrentUserChanged.listen((
        GoogleSignInAccount? googleAccount) async {
      final account = await HookHook.backend.service.addGoogle(
          googleAccount!.serverAuthCode!);
      if (account != null) {
        setState(() {
          accounts.add(account);
        });
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
          backgroundColor: const Color(0xFFF8CBAA),
          title: const Center(
              child: Text(
                  "Google",
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
                                    "Google", e.userId);
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
            /*Padding(
              padding: const EdgeInsets.symmetric(
                  horizontal: 50,
                  vertical: 8
              ),
              child: TextButton(
                  onPressed: () async {
                    try {
                      await google.signIn();
                    } catch (error) {
                      if (kDebugMode) {
                        print(error);
                      }
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
            )*/
          ]
      );
}
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

class Github extends StatefulWidget {
  final double width;
  final bool enabled;
  const Github({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Github createState() => _Github();
}

class _Github extends AdaptiveState<Github> {
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
                builder: (context) => const GithubDialog()
            );
          } : null,
        )
    );
  }
}

class GithubDialog extends StatefulWidget {
  const GithubDialog({Key? key}) : super(key: key);

  @override
  _GithubDialog createState() => _GithubDialog();
}

class _GithubDialog extends AdaptiveState<GithubDialog> {
  late StreamSubscription listener;
  List<Account> accounts = [];

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Github").then((value) =>
        setState(() {
          accounts = value;
        })
    );
    listener = linkStream.listen((String? response) async {
      if (response!.startsWith(dotenv.env["GITHUB_REDIRECT"]!)) {
        final url = Uri.parse(response);
        final account = await HookHook.backend.service.addGitHub(
            url.queryParameters["code"]!);
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
          backgroundColor: const Color(0xFFF5CDCB),
          title: const Center(
              child: Text(
                  "GitHub",
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
                                    "GitHub", e.userId);
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
                      "user",
                      "repo"
                    ];
                    await redirect(
                        "https://github.com/login/oauth/authorize?client_id=${dotenv
                            .env["GITHUB_CLIENTID"]}&redirect_uri=${dotenv
                            .env["GITHUB_REDIRECT"]}&response_type=code&scope=${scopes
                            .join(' ')}");
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
                    ),
                  )
              ),
            )
          ]
      );

}
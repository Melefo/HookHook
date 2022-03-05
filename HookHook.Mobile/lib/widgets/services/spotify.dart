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

class Spotify extends StatefulWidget {
  final double width;
  final bool enabled;
  const Spotify({Key? key, required this.width, this.enabled = false}) : super(key: key);

  @override
  _Spotify createState() => _Spotify();
}

class _Spotify extends AdaptiveState<Spotify> {
  @override
  Widget build(BuildContext context) {
    return ListItem(
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
                builder: (context) => const SpotifyDialog()
            );
          } : null,
        )
    );
  }
}

class SpotifyDialog extends StatefulWidget {
  const SpotifyDialog({Key? key}) : super(key: key);

  @override
  _SpotifyDialog createState() => _SpotifyDialog();
}

class _SpotifyDialog extends AdaptiveState<SpotifyDialog> {
  late StreamSubscription listener;
  List<Account> accounts = [];

  @override
  void initState() {
    super.initState();
    HookHook.backend.service.getAccounts("Spotify").then((value) =>
        setState(() {
          accounts = value;
        })
    );
    listener = linkStream.listen((String? response) async {
      if (response!.startsWith(dotenv.env["SPOTIFY_REDIRECT"]!)) {
        final url = Uri.parse(response);
        final account = await HookHook.backend.service.addSpotify(url.queryParameters["code"]!);
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
          backgroundColor: const Color(0xFFB4E1DC),
          title: const Center(
              child: Text(
                  "Spotify",
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
                                    "Spotify", e.userId);
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
                      "user-read-email",
                      "user-read-private",
                      "user-library-modify",
                      "user-library-read",
                      "playlist-modify-private",
                      "playlist-read-private",
                      "playlist-modify-public",
                    ];
                    await redirect("https://accounts.spotify.com/authorize?client_id=${dotenv.env['SPOTIFY_CLIENTID']}&redirect_uri=${dotenv.env['SPOTIFY_REDIRECT']}&response_type=code&scope=${scopes.join(" ")}");
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
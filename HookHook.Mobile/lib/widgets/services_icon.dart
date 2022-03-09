import 'package:flutter/material.dart';
import 'package:hookhook/widgets/services/discord.dart';
import 'package:hookhook/widgets/services/github.dart';
import 'package:hookhook/widgets/services/google.dart';
import 'package:hookhook/widgets/services/spotify.dart';
import 'package:hookhook/widgets/services/twitch.dart';
import 'package:hookhook/widgets/services/twitter.dart';

class ServiceIcon {
  static Widget serviceIcon(String provider) {
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

}
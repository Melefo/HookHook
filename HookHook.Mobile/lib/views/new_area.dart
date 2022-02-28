import 'package:flutter/material.dart';
import 'package:hookhook/services_icons.dart';
import 'package:hookhook/wrapper/backend.dart';

class NewAreaView extends StatelessWidget {

  static String routeName = "/new_area";

  const NewAreaView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          body: Padding(
            padding: const EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 80
            ),
            child: Container(
              child: Column(
                children: [
                  TextButton(
                    onPressed: () {
                      Navigator.pop(context);
                    },
                    child: const Padding(
                      padding: EdgeInsets.fromLTRB(120.0, 15.0, 120.0, 15.0),
                      child: Text(
                        "New Area",
                        style: TextStyle(
                            fontSize: 20,
                            color: Colors.black,
                            fontWeight: FontWeight.w500),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          )
      );
}
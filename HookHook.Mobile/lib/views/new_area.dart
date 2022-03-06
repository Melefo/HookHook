import 'package:flutter/material.dart';

import '../hookhook_colors.dart';

class NewAreaView extends StatelessWidget {

  static String routeName = "/new_area";

  const NewAreaView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: HookHookColors.light,
          body: Padding(
            padding: const EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 80
            ),
            child: Column(
              children: [
                TextFormField(
                    decoration: const InputDecoration(
                        border: UnderlineInputBorder(),
                        labelText: "Area Name"
                    )
                ),
                Padding(
                  padding: const EdgeInsets.only(top: 20),
                  child: Center(
                    child: Container(
                      decoration: const BoxDecoration(
                          borderRadius: BorderRadius.all(Radius.circular(25)),
                          color: Colors.white
                      ),
                      child: TextButton(
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
                    ),
                  ),
                ),
              ],
            ),
          )
      );
}
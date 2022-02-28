import 'package:flutter/material.dart';
import 'package:hookhook/hookhook_colors.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';

class AreaItem extends StatelessWidget {

  final String areaName;
  final String action;
  final String reaction;


  const AreaItem({Key? key, this.areaName = "Area Name", this.action  = "default", this.reaction  = "default"}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Container(
          alignment: Alignment.topCenter,
          padding: const EdgeInsets.all(10.0),
          height: 180,
          width: 350,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.all(Radius.circular(25)),
            color: Colors.white,
          ),
          child: Column(
            children: [
              Text(areaName, style: TextStyle(color: Colors.black)),
              Padding(
                padding: const EdgeInsets.only(top: 30),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    Icon(Icons.category, color: HookHookColors.blue),
                    Icon(Icons.arrow_right_alt_rounded, color: Colors.black),
                    Icon(Icons.category, color: HookHookColors.orange)
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(top: 20, right: 5, left: 5),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: <Widget> [
                    Text('14h03 : 15 decembre', style: TextStyle(color: Colors.white)),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(top: 6),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    Container(
                      decoration: BoxDecoration(
                          borderRadius: BorderRadius.all(Radius.circular(10)),
                          color: HookHookColors.light
                      ),
                      child: IconButton(
                        onPressed: () {
                          print("Press");
                        },
                        icon: Icon(Icons.refresh, color: Colors.black),
                      ),
                    ),
                    Container(
                      decoration: BoxDecoration(
                          borderRadius: BorderRadius.all(Radius.circular(10)),
                          color: HookHookColors.light,
                      ),
                      child: IconButton(
                        onPressed: () {
                          print("Press");
                        },
                        icon: Icon(Icons.delete, color: Colors.black),
                      ),
                    )
                  ],
                ),
              ),

            ],
          ),
        ),
      ),
    );
  }

}
import 'package:hookhook/widgets/list_items.dart';
import 'package:mvc_application/controller.dart';
import 'package:mvc_application/view.dart';
import 'package:flutter/material.dart';
import 'package:hookhook/widgets/h_list.dart';

//view
class HomeView extends StatefulWidget {

  static String routeName = "/";

  const HomeView({Key? key}) : super(key: key);

  @override
  StateMVC<HomeView> createState() => _Home();
}

//state
class _Home extends StateMVC<HomeView> {

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          body: Column(
            children: <Widget>[
              const Padding(
                padding: EdgeInsets.only(top: 70),
                child: Text('HookHook', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 45)),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 30, 0, 0),
                child: Text('Hello, Arthur', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 30)),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 20),
                child: Text('Welcome back !', style: TextStyle(fontSize: 30)),
              ),
              const HList(Widgets:
              <Widget>[
                ListItem(cornerRadius: 25, color: Color(0xFFA3E7EE), Content: Icon(Icons.category, color: Color(0xFF73B6BD), size: 70)),
                ListItem(cornerRadius: 25, color: Color(0xFFB4E1DC), Content: Icon(Icons.category, color: Color(0xFF85B1AC), size: 70)),
                ListItem(cornerRadius: 25, color: Color(0xFFF5CDCB), Content: Icon(Icons.category, color: Color(0xFFC49E9C), size: 70)),
                ListItem(cornerRadius: 25, color: Color(0xFFF8CBAA), Content: Icon(Icons.category, color: Color(0xFFC79D7D), size: 70)),
                ListItem(cornerRadius: 25, color: Color(0xFFFFFFC7), Content: Icon(Icons.category, color: Color(0xFFC6C791), size: 70))
              ]
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Creator', style: TextStyle(fontFamily: 'Comfortaa', fontSize: 15)),
              ),
              ClipRRect(
                borderRadius: BorderRadius.circular(30.0),
                child: Container(
                  color: Color(0xFF3B3F43),
                  width: 350,
                  height: 300,
                ),
              ),
              const Padding(
                padding: EdgeInsets.fromLTRB(0, 10, 0, 10),
                child: Text('Your AREAs', textAlign: TextAlign.left, style: TextStyle(fontFamily: 'Comfortaa', fontSize: 15)),
              ),
              const HList(Widgets:
                <Widget>[
                  ListItem(cornerRadius: 20, color: Color(0xFF3B3F43), Content: Icon(Icons.category, color: Colors.orange,)),
                  ListItem(cornerRadius: 20, color: Color(0xFF3B3F43), Content: Icon(Icons.category, color: Colors.orange,)),
                  ListItem(cornerRadius: 20, color: Color(0xFF3B3F43), Content: Icon(Icons.category, color: Colors.orange,)),
                  ListItem(cornerRadius: 20, color: Color(0xFF3B3F43), Content: Icon(Icons.category, color: Colors.orange,)),
                  ListItem(cornerRadius: 20, color: Color(0xFF3B3F43), Content: Icon(Icons.category, color: Colors.orange,))
                ]
              ),
            ],
          ),
      );

  Widget _buildBox({required Color color}) => Container(margin: EdgeInsets.all(12), height: 100, width: 100, color: color);
}
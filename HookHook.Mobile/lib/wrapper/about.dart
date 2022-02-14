import 'package:flutter/cupertino.dart';
import 'package:http/http.dart' as http;

class Action
{
  final String Name;
  final String Description;

  Action(this.Name, this.Description);
}

class Reaction
{
  final String Name;
  final String Description;

  Reaction(this.Name, this.Description);
}

class Services
{
  final String Name;
  final <Action>[] Actions;
  final <Reaction>[] Reactions;

  Services(this.Name);
}

class Server
{
  final CurrentTime;
  final <Services>[] Services;

  Server(this.CurrentTime);
}

class Client
{
  final String Host;

  Client(this.Host);
}

class About
{
  final String _url = String.fromEnvironment('BACKEND_URL', defaultValue: 'http://localhost:8080');

  final Client client;
  final Server server;

  About(this.client, this.server);

  Future<void> fetch() async {
    var res = await http.get(Uri.parse(_url + '/about.json'));
    print(res.body);
  }
}
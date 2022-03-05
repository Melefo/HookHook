class Account {
  Account({
    required this.userId,
    required this.username,
  });

  final String userId;
  final String username;

  factory Account.fromJson(Map<String, dynamic> json) => Account(
    username: json["username"],
    userId: json["userId"],
  );

  Map<String, dynamic> toJson() => {
    "userId": userId,
    "username": username,
  };
}

class Service {
  Future<List<Account>> getAccount() async {
    return [];
  }
}
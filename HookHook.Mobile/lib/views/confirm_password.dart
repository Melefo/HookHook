import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:hookhook/adaptive_state.dart';
import 'package:hookhook/widgets/welcome_hookhook.dart';
import '../hookhook_colors.dart';
import '../main.dart';

class ConfirmPassword extends StatefulWidget {
  final String id;

  const ConfirmPassword({Key? key, required this.id}) : super(key: key);

  static String routeName = "/confirm/";

  @override
  _ConfirmPassword createState() =>
      _ConfirmPassword();
}

class _ConfirmPassword extends AdaptiveState<ConfirmPassword> {
  TextEditingController password = TextEditingController();
  TextEditingController confirm = TextEditingController();
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  Future<void> validateAndSave() async {
    final FormState form = _formKey.currentState!;
    if (!form.validate()) {
      return;
    }
    await HookHook.backend.signIn.confirmPassword(
        widget.id, password.value.text);
    password.clear();
    confirm.clear();
    await SystemNavigator.pop(animated: true);
  }

  @override
  Widget build(BuildContext context) =>
      Scaffold(
          backgroundColor: darkMode ? HookHookColors.dark : HookHookColors
              .light,
          body: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  Image.asset(
                      "assets/pinguin/mop.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  ),
                  const WelcomeHookHook(),
                  Image.asset(
                      "assets/pinguin/mop.gif",
                      height: 0.15.sw,
                      width: 0.15.sw
                  )
                ],
              ),
              Text(
                  "Enter your new password.",
                  style: TextStyle(
                      fontSize: 14.sp,
                      color: darkMode ? Colors.white : Colors.black
                  )
              ),
              Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 0.15.sw
                ),
                child: Form(
                  key: _formKey,
                  child: Column(
                    children: [
                      TextFormField(
                          decoration: InputDecoration(
                            enabledBorder: UnderlineInputBorder(
                                borderSide: BorderSide(
                                    color: darkMode ? Colors.white : Colors
                                        .black
                                )
                            ),
                            labelText: "Password",
                            labelStyle: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          controller: password,
                          obscureText: true,
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                          validator: (text) {
                            if (text == null || text.length < 4) {
                              return "Password must be at least 4 characters";
                            }
                            if (text.length > 256) {
                              return "Password cannot exceed 256 characters";
                            }
                            return null;
                          }
                      ),
                      TextFormField(
                          decoration: InputDecoration(
                            enabledBorder: UnderlineInputBorder(
                                borderSide: BorderSide(
                                    color: darkMode ? Colors.white : Colors
                                        .black
                                )
                            ),
                            labelText: "Confirm",
                            labelStyle: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          controller: confirm,
                          obscureText: true,
                          style: TextStyle(
                              color: darkMode ? Colors.white : Colors.black
                          ),
                          validator: (text) {
                            if (text != password.value.text) {
                              return "Doesn't match password";
                            }
                            return null;
                          }
                      ),
                      const Padding(
                        padding: EdgeInsets.all(8),
                      ),
                      TextButton(
                          onPressed: () async => await validateAndSave(),
                          child: Text(
                            "Send",
                            style: TextStyle(
                                color: darkMode ? Colors.white : Colors.black
                            ),
                          ),
                          style: TextButton.styleFrom(
                              backgroundColor: darkMode
                                  ? HookHookColors.gray
                                  : Colors.white,
                              padding: const EdgeInsets.all(15),
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10)
                              ),
                              minimumSize: const Size(150, 0)
                          )
                      ),
                    ],
                  ),
                ),
              ),
            ],
          )
      );
}
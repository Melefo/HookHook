FROM ubuntu

ENV DEBIAN_FRONTEND=noninteractive 

#Installing deps
RUN apt update
RUN apt install -qqy --no-install-recommends curl git unzip xz-utils zip libglu1-mesa openjdk-13-jdk wget

#Installing Android SDK
RUN mkdir -p /android/sdk/cmdline-tools/tools
ENV ANDROID_SDK_ROOT /android/sdk

RUN wget -O cmdline-tools.zip https://dl.google.com/android/repository/commandlinetools-linux-7583922_latest.zip
RUN unzip cmdline-tools.zip && rm cmdline-tools.zip
RUN mv cmdline-tools/* android/sdk/cmdline-tools/tools

RUN cd /android/sdk/cmdline-tools/tools/bin && yes | ./sdkmanager --licenses
ENV PATH "$PATH:/android/sdk/platform-tools"

#Installing Flutter
RUN git clone https://github.com/flutter/flutter.git -b stable
ENV PATH="$PATH:/flutter/bin"

RUN flutter doctor

#Installing app
WORKDIR /app
COPY ["HookHook.Mobile", "/app"]

RUN flutter pub get
RUN flutter analyze
RUN flutter build apk

WORKDIR /build
RUN cp /app/build/app/outputs/flutter-apk/app-release.apk /build/client.apk
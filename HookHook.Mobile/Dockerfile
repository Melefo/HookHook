FROM androidsdk/android-31:latest as build
WORKDIR /

RUN git clone https://github.com/flutter/flutter.git

RUN git -C flutter checkout stable
ENV PATH "/flutter/bin:$PATH"

RUN flutter upgrade && flutter doctor

WORKDIR /app
COPY ["HookHook.Mobile", "/app"]

RUN flutter pub get
RUN flutter build apk

FROM alpine as app
WORKDIR /build

COPY --from=build /app/build/app/outputs/flutter-apk/app-release.apk /build/client.apk
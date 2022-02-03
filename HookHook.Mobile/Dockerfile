FROM itachi1706/flutter-alpine-android:latest as build

#Installing app
WORKDIR /app
COPY ["HookHook.Mobile", "/app"]

RUN flutter pub get
RUN flutter build apk

FROM scratch as app

WORKDIR /build
COPY --from=build /app/build/app/outputs/flutter-apk/app-release.apk /build/client.apk
FROM mingc/android-build-box:latest as build

#Installing app
WORKDIR /app
COPY ["HookHook.Mobile", "/app"]

RUN flutter pub get
RUN flutter build apk

FROM alpine as app
WORKDIR /build

COPY --from=build /app/build/app/outputs/flutter-apk/app-release.apk /build/client.apk

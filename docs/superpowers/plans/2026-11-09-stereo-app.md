# REALMAX Stereo App Implementation Plan

> Execute inline using executing-plans. User explicitly requested implementation without further planning handoffs.

**Goal:** Install an independent stereo AR application using the factory SVR and controller interfaces, with spatial browser and cloud AI panels.
**Architecture:** Built-in Unity rendering through original SvrCamera; native Android browser engine renders into a Unity texture; controller ray UV coordinates become browser touch events. A separate provider adapter handles HTTPS AI requests and settings.
**Tech Stack:** Available Unity 6000.3.27f1, Android API 25 device, ARMv7 factory SVR libraries, C# and Java.
**Spec:** ../../development-context.md

## Global Constraints

- Independent package com.ar.realmaxworkspace; preserve factory packages, services and calibration.
- Device standalone; no computer streaming required for product operation.
- Keep speaker muted. Existing backups remain intact; no firmware changes.
- Retain TLS checks. Do not embed provider secrets in source, logs or Git.

## Review Focus

- Native SVR initialization and pause/resume after returning from Android UI.
- Controller gaze/ray targets and texture UV-to-pixel mapping at borders.
- Browser engine compatibility, video textures, and permission lifecycle.
- Missing AI configuration must show a useful message, not silently pretend success.
- Existing IStar service interface differences; do not replace factory services.

### Task 1: Build and install stereo host
- [ ] Create isolated app/ project in current clean repository; original source archives remain untouched.
- [ ] Extract SDK assets from verified original package, adapt only build incompatibilities in project copy.
- [ ] Add reproducible scene/build script, world-space panel and runtime diagnostics.
- [ ] Build APK and validate installed package and native SVR runtime logs on device.

### Task 2: Spatial interaction
- [ ] Test UV-to-pixel mapping on corners, center, and clamping before implementation.
- [ ] Bind factory IControllerPointHandle and ControllerKeyEvent to spatial actions.
- [ ] Demonstrate panel action through real controller or explicitly document unverified input.

### Task 3: Browser and AI surfaces
- [ ] Integrate native browser-to-texture bridge; assess GeckoView against API 25 and ARMv7 host.
- [ ] Add web navigation and controller click/scroll forwarding.
- [ ] Add configurable HTTPS provider endpoint/model and text question UI, handling unconfigured/error states.
- [ ] Verify Bilibili rendering separately from video playback; do not claim playback without evidence.

### Task 4: Verify and deliver
- [ ] Run behavioral checks and Android build, inspect native logs and dual-eye configuration.
- [ ] Review code, record limitations, commit sources/build instructions and APK hash.
- [ ] Install only independent app and record rollback command.

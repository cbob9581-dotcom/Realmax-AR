# Execution ledger — plan: docs/superpowers/plans/2026-11-09-stereo-app.md

User requested direct implementation. Work in clean current checkout with new app/ directory; factory archives and APKs remain untouched. No additional worktree, no device firmware mutations.
Task 1: in progress — building factory SDK on available Unity 6 first; native SVR compatibility requires real device validation.

Task 2: coordinate tests RED (not implemented) -> GREEN, six checks passed.
Task 3: provider validation tests RED -> GREEN, seven checks passed; spatial keyboard and encrypted settings implemented.
Task 1: Android library resource migration complete; Gradle reached Java compilation. GeckoView 143 SurfaceInfo signature corrected from actual artifact bytecode.

Task 1: first complete APK built successfully in unity-build7.log (161766958 bytes). Not installed before review fixes.
Final review: one independent reviewer found input-name log leakage, mutable provider parsing, Gemini model usage, hidden browser keyboard, background pause, system-wide audio mutation, and pagination overshoot. All entered fix pass; no code changes to factory device services.
Final: RED security test in actual Unity reproduced SECRET_CANARY in action log. Fix removes raw names; build now runs ArChecks.Run before packaging.
Final: WorkspaceRules tests 8/8 RED->GREEN cover endpoint model selection, page clamping and browser activity state. Session MediaSession.muteAudio API verified against actual GeckoView143 artifact.

Final: security regression verified RED in actual Unity, then GREEN in unity-build8.log. SpatialAction no longer records object/character names.
Final: request adapter/model/key captured before network yield; redirects disabled to prevent credential forwarding. Gemini endpoint uses model selection.
Final: browser visibility plus application pause controls Gecko active state; Home no longer opens hidden page keyboard. Reply page index clamps to last page.
Final: Gecko MediaSession callbacks mute audio, removing app-initiated global volume writes. Unity AudioListener remains zero.
Deferred: physical optics/input orientation and live cloud API success remain device/user validations; source review cannot establish them.

Device RED: first installed build SIGABRT in libsvrplugin SvrCanBeginVR. Tombstone proves legacy JNI expects Activity.mUnityPlayer descriptor Lcom/unity3d/player/UnityPlayer; plus UnityPlayer.getChildAt(int). Unity6 Activity uses UnityPlayerForActivityOrService and player no longer extends FrameLayout. Implemented independent RealmaxActivity plus CompatUnityPlayer exposing exact legacy field and SurfaceView getter, retaining Unity6 lifecycle delegation. No firmware or factory service replacement.

Device GREEN: 0.1.1 pure stereo host runs continuously, SVR RUNNING pose=3, L/R offsets -0.03/+0.03, both 1440x1440 textures captured. JNI compatibility fixed.
Device findings: factory main cameras exclude UI layer 5; eye captures showed reticle only. Explicitly include workspace UI and all eye culling layers.
Device findings: Gecko producer outputs RGBX(0x2), ImageReader RGBA(0x1) caused UnsupportedOperationException. Change reader to RGBX.
Device findings: Gecko AudioIPC SIGSEGV in development build. Primary embedding maintainer documents Development Build and Mono signal-handler compatibility; use non-development player and MOZ_DISABLE_SIG_HANDLER=1 (crash/signal interop only, not TLS or content sandbox changes). Source: https://support.vuplex.com/articles/android-gecko-crash/ and https://developer.vuplex.com/webview/releases .

Device GREEN (0.1.2): build11 succeeded, installed replacement APK versionCode3. PID30848 remained live for over two minutes (19:00–19:03 local), SVR RUNNING pose3, first Gecko frame received, no crash-buffer entry for that PID. Both 1440x1440 eye captures now visibly contain the workspace controls and live Bilibili HTTPS desktop homepage; L/R panel positions differ as expected for separate eye cameras. Captures: .backups/runtime-0.1.2/diagnostics/{left,right}-eye.png. This establishes actual stereo texture rendering, not wearer optical comfort, controller activation, Bilibili playback or login.
Release 0.1.2 SHA256: 495BAB1C7EA25F1F75428D2524D76E84756F4B8338DBA15DA2DB9BE0D21528DD. Immutable APK: .backups/releases/0.1.2/RealmaxWorkspace.apk.
Device audio verification: STREAM_MUSIC muted=true, speaker index0. Device app memory PSS approximately488MB during browser homepage.
Controller: original bonded realmax-C157 remains registered. Factory ConnectionIStar bridge retained, location runtime permission granted; current callback remains WAITING. User confirmation/physical input required; no hardware failure inferred.
Remaining validation: physical hand controller click/ray, wearer alignment/recentering, Bilibili playback/login, cloud provider real response after configuration. Camera/microphone are hardware-verified separately but not wired to cloud vision/speech in this version. Standalone app performs all Unity/Gecko rendering locally on headset; PC hotspot currently supplies WiFi.

Controller update 20:36 local: user confirmed power ON. Restarted only workspace app to issue fresh factory connectGatt. Service discovery status=0, battery and factory data notification characteristics registered. Unity PID13158 now reports controller CONNECTED repeatedly. No source/device firmware change required. Root cause of prior stale connection not established. Physical ray/button test requested; awaiting user observation. Prior browser PID30848 was still running at20:35 (over90 minutes after initial launch).

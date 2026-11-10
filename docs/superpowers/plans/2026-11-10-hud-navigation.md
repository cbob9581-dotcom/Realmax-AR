# Glowing transparent HUD and reliable return navigation

> Execute inline with superpowers:executing-plans; user requests implementation from attached reference and prior direct-execution preference remains applicable.

Goal: luminous ice-blue typography, restrained rings/lines, no filled menu panels, and consistent exits from every workspace page. Preserve binocular runtime, hand/gaze, silence, credentials and manual networking.
Architecture: common SpatialUi plus bounded mesh glow and procedural HUD strokes; pure WorkspaceNavigation history separates workspace Back from browser history. Persistent top Back/Home/Recenter above modal keyboards. Menu = Home/recenter; Return = previous page/recenter. Pending requests are invalidated/cancelled on navigation.
Files: HudGlow.cs/HudGraphic.cs for visual geometry; WorkspaceNavigation.cs and tests for history; ArWorkspace and JarvisAgent integrate routes; JarvisPanel/SpatialUi/keyboards/AI use transparent shared styling; HudDiagnostics captures real binocular UI states and asserts route transitions.
- [x] RED/GREEN navigation tests: nested pages, root, transient result, restoring answer and repeated home.
- [x] Implement shared glow and transparent homepage/menu/input styling; retain invisible modal input blockers.
- [x] Add persistent exits and controller/system Back, cancel stale requests and close keyboards before leaving parent page.
- [x] Independent final review and Android compile checks; native packaging and device harness continue below.
- [x] Build/backup/install; real headset route diagnostics and both-eye inspection PASS; HOME and silence verified.
- [x] Upload final source/APK backup: v0.3.0-rc1.
Limits: other Android apps/system settings retain their native UI; website content retains site rendering. The reference is styling inspiration, not camera imagery or a background asset.

# 2026-11-09 浏览器与音视频基础验证

本轮是实机诊断，不是完成的 AR 浏览器或 AI 产品。

## 安装记录

- 原始状态：设备未安装 org.mozilla.firefox。
- 安装：Firefox 143.0.4，versionCode 2016118130，ARM64，minSdk 21，targetSdk 36。
- 来源：https://archive.mozilla.org/pub/fenix/releases/143.0.4/android/fenix-143.0.4-android-arm64-v8a/fenix-143.0.4.multi.android-arm64-v8a.apk
- APK：`.backups/downloads/fenix-143.0.4.multi.android-arm64-v8a.apk`
- APK SHA-256：20a7540c2118c27e0edafc009597ad67fb3b6ea7fa34e6bde4fc100ea92197aa
- Android apksigner 验证退出码 0；签名证书 SHA-256：a78b62a5165b4494b2fead9e76a280d22d937fee6251aece599446b2ea319b04。
- 安装返回 Success，包查询与运行进程确认 ARM64 Firefox 启动成功。
- 原厂应用和系统分区未被覆盖，未改默认启动器。

## 相机与麦克风

本地诊断页 `tools/capability_probe.py` 临时监听电脑 127.0.0.1:8765，通过 USB adb reverse 访问；仅测试用途，不能据此声称独立 AI 产品已完成。

- 用户触发相机测试，Firefox 提示授权；未勾选记住决定。
- 相机 0（Facing back:0）返回活动视频帧，360x480，轨道设置 frameRate=30。
- 测试后轨道停止；CameraService 复查 Active Camera Clients 为空。
- 麦克风返回活动轨道，采样率 48000 Hz，peak=0.04982276260852814，rms=0.00618492077885429，检测到非零输入。
- 麦克风只在内存中计算约一秒幅度，无音频文件，没有连接到扬声器输出。
- 未保存或向云端上传相机帧或录音；仅保存数值与元数据结果。
- 日志见 `docs/runs/*-camera.json` 和 `docs/runs/*-microphone.json`。
- Firefox 默认报告的 User-Agent 含 Android 10；设备实机仍为 Android 7.1.2。没有手工覆写 User-Agent。
- 尚未验证与 6DoF 追踪同时使用相机；未验证拾音质量、语音识别或云端视觉模型。

## B 站

- 设备 HTTPS 请求返回 200。
- Firefox 实际加载 B 站移动首页与公开视频详情，包括缩略图、标题和相关视频。
- 移动详情页出现打开 App 引导；尚未取得视频持续播放的证据。
- 桌面模式尝试中出现 ADB 地址输入残留，访问到了错误 URL；不能把该错误页归因为浏览器不兼容或网站风控。结束时重新打开准确的移动首页。
- 未登录账户，未测试会员、DRM、高清、弹幕与长时间播放。
- 普通网页显示尚未接入 RealMax 双目渲染，也未完成手柄到网页输入的映射。

## 回退

Firefox 是新安装的独立包。若确认不需要保留其后续浏览数据，可卸载 org.mozilla.firefox 还原为安装前无该包的状态；该操作会删除该浏览器数据，执行前应再次核对。

临时 USB 转发在测试结束后移除，关闭本地服务；无需更改系统证书或禁用 HTTPS 校验。测试保持媒体音量 0、Muted=true。

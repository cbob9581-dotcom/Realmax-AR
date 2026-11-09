# REALMAX Workspace

独立包名：`com.ar.realmaxworkspace`。这是使用原厂 SVR 左右眼渲染、工厂手柄接口的 Unity Android 应用，网页通过 GeckoView 离屏纹理显示在空间面板中。

## 构建

本机编辑器：`D:/Unity/6000.3.27f1/Editor/Unity.exe`。

```powershell
& 'D:/Unity/6000.3.27f1/Editor/Unity.exe' -batchmode -nographics -projectPath 'F:/AR/app' -executeMethod ArBuild.Build -quit -logFile 'F:/AR/.backups/build.log'
```

也可打开工程，使用菜单 `REALMAX > Build Workspace APK`。产物为 `Builds/RealmaxWorkspace.apk`。

第一次构建会从 Mozilla Maven 下载 GeckoView 143.0.20251003115653 及公开依赖。项目 `.dependencies/maven` 可作本地缓存，不进入 Git。`ArBuild` 自动生成本机 Gradle 仓库模板，模板不进入 Git。

## 操作

- Home：空间主页。
- Browser：在左右眼空间面板中打开 B 站，使用 Gecko 原生桌面网站模式。
- AI：打开云端配置、提问和空间键盘。
- 手柄射线指向按钮并按触摸板触发；原厂头瞄停留触发仍保留。
- Scroll up/down：网页滚动；AI 页面则翻页阅读回答。
- Keyboard / Recenter：浏览器可见时打开空间网页键盘，否则将面板移回视野前方。
- 网页键盘：Open URL 输入 HTTPS 地址；Type into page 将输入发送到刚刚选中的网页输入框。

## AI 配置

在 AI 面板选择 Endpoint、Model、API key 或 Question 后，通过空间键盘输入。

- `openai-compatible`：Endpoint 填完整 chat-completions 兼容接口 URL。
- `anthropic`：Endpoint 填完整 messages 接口 URL。
- `gemini`：Endpoint 可填 API 版本根地址或已有 models/...:generateContent 地址；Model 会应用到请求路径。
- Save settings：使用 Android Keystore AES-GCM 加密并保存到应用私有设置。
- Send question：发送文字请求。尚未配置时不会发送请求。

未提供云端账户或密钥，尚未验证真实云端成功响应。不要将密钥填入源码、提交到 Git 或发送到日志中。

## 已知范围

- 浏览器纹理当前采用 CPU 读回，约 8–10 次/秒更新，尚不是为高清视频优化的 GPU 共享纹理路径。
- B 站登录、播放、会员/DRM 和长时间运行须分别实测；不能由网页加载推断全部支持。
- 此版 AI 为文字问答；此前相机/麦克风硬件已单独验证，尚未接入本应用的云端图像/语音请求。
- 当前应用静音：Unity 音量为零，Gecko 媒体会话设置 muteAudio，不修改用户系统音量。
- 工厂 SDK 核心库仅 ARMv7，因此应用构建 ARMv7，Gecko 采用兼容 Android 7 的 143 版本。

## 诊断与回退

开发启动参数 `--es startPage browser --ez diagnostics true` 会自动打开空间网页，并在约 12 秒后将左右眼纹理保存到应用自己的 `files/diagnostics`；普通启动不截图。

新安装独立包不会覆盖原厂应用。卸载 `com.ar.realmaxworkspace` 可以移除本应用，但会同时删除该应用设置与加密密钥；需要保留的数据应先另行保存。原厂应用和系统分区未修改。

## 0.1.2 设备验证（2026-11-09）

已安装到当前眼镜。左右眼各 1440×1440 的实际输出均显示空间控制面板和 B 站首页，应用持续运行超过两分钟，SVR 姿态状态恢复为 3。先前的 Unity6/原厂 JNI、UI 图层不可见及 Gecko 读取格式/启动崩溃问题在该运行中未再出现。

手柄已有蓝牙配对记录，但本应用仍显示 WAITING，尚未实测点击。佩戴融合、手柄射线、视频播放和登录需继续现场验证。当前不是完整固件备份，也未更改系统固件。

保留安装包：`F:/AR/.backups/releases/0.1.2/RealmaxWorkspace.apk`。
SHA256：`495BAB1C7EA25F1F75428D2524D76E84756F4B8338DBA15DA2DB9BE0D21528DD`。

# REALMAX AR Workspace

REALMAX 乾眼镜上的独立双目 AR 工作台，Unity + 原厂 SVR 渲染，内嵌 GeckoView 浏览器。眼镜本机运行，通过 Wi-Fi 联网，不依赖电脑串流。

## 当前已验证版本

`0.1.4-r1`（Android versionName 0.1.4 / versionCode 6）。

- 真实重启后自动进入双目工作台；Home 键进入工作台。
- B 站 HTTPS 联网返回 200，左右眼纹理均显示网页。
- 保留原厂蓝牙手柄与头部准星；区分网页点击的两种射线来源。
- 固定距离模式关闭位置追踪，保留双目与转头；菜单键召回面板。
- 云端 AI 文字问答支持兼容接口、Anthropic、Gemini，密钥使用 Android Keystore 加密，等待用户配置。
- 全程静音。摄像头和麦克风硬件此前单独验证，未接入云端图像或语音上传。

## 已知限制

当前网页使用 CPU 纹理读回，约 8 帧/秒，用户反馈清晰度、流畅度和准星对比度仍需改善。真实云端响应、视频播放/登录和佩戴舒适度不能由页面加载推断。

系统设置和其他 Android 软件保留其原有显示方式；本工作台不会自动将整个 Android 系统转换为双目。每次更新 APK 后需要重新核对默认 HOME 设置。

“贾维斯”语音智能体是接下来的开发阶段：本版本尚无语音唤起功能。未完成的 GPU 实验保留在本地，不包含于该已验证源码快照。

## 工程与构建

工程位于 `app/`，详细说明见 [app/README.md](app/README.md)。本机使用 Unity 6000.3.27f1、Android API25、ARMv7、GLES3。原厂 SDK 核心库限定 ARMv7。

```powershell
& 'D:/Unity/6000.3.27f1/Editor/Unity.exe' -batchmode -nographics -projectPath 'F:/AR/app' -executeMethod ArBuild.Build -quit -logFile 'F:/AR/.backups/build.log'
```

首次构建需要从官方 Mozilla Maven 下载 GeckoView 143 及公开依赖。依赖缓存、Unity 生成目录、设备备份和配置密钥不进入 Git。

## 备份与验证记录

- [开发背景](docs/development-context.md)
- [恢复说明](docs/backup-and-recovery.md)
- [执行与验证记录](docs/implementation/progress.md)

原始归档、系统关键 APK 和各次安装包保留在开发电脑的 `.backups/`，不上传仓库。未刷写固件或修改原厂校准；这不等同于完整固件恢复备份。
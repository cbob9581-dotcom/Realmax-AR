# REALMAX AR Workspace

REALMAX 乾眼镜上的独立双目 AR 工作台，Unity + 原厂 SVR 渲染，内嵌 GeckoView 浏览器。眼镜本机运行，通过 Wi-Fi 联网，不依赖电脑串流。

## 浏览器历史基线

`0.1.4-r1`（Android versionName 0.1.4 / versionCode 6）。

- 真实重启后自动进入双目工作台；Home 键进入工作台。
- B 站 HTTPS 联网返回 200，左右眼纹理均显示网页。
- 保留原厂蓝牙手柄与头部准星；区分网页点击的两种射线来源。
- 固定距离模式关闭位置追踪，保留双目与转头；菜单键召回面板。
- 云端 AI 文字问答支持兼容接口、Anthropic、Gemini，密钥使用 Android Keystore 加密，可在眼镜中配置。
- 全程静音。摄像头和麦克风硬件此前单独验证，此历史版本未接入云端图像或语音上传。

## 已知限制

网页仍使用 CPU 纹理读回，约 8 帧/秒，用户反馈清晰度、流畅度和准星对比度仍需改善。真实云端响应、视频播放/登录和佩戴舒适度不能由页面加载推断。

系统设置和其他 Android 软件保留其原有显示方式；本工作台不会自动将整个 Android 系统转换为双目。每次更新 APK 后需要重新核对默认 HOME 设置。

## 贾维斯候选版本 0.2.0-rc1（versionCode 14）

- 本机 Vosk 中文离线唤起与识别，不上传麦克风录音，不播放声音。Android 实机已通过静默合成语音“假 为师 打开 软件 列表”的识别与打开列表测试；这不代表真实佩戴者语音准确率已验证。
- 软件列表、设置、电量、网络、本机软件打开，以及配置云端后的自然语言问答；云端操作受严格允许列表限制。手柄和准星保留，Wi-Fi 密码手动输入。
- 已使用眼镜端加密保存的 DeepSeek 配置，真实请求得到 HTTP200 和 get_status 操作。密钥不进入仓库或命令行参数。
- 回答页按用户要求使用透亮科技蓝大字，无可见背景底板；现实世界透过光学眼镜可见。长回答支持无底色的翻页按钮。
- 明确说“贾维斯，眼前有什么”或点“拍照问答”才从 RGB 相机0取单张照片，释放相机后发给配置的视觉模型。相机1不作为后备。图片回答只显示文字，不执行工具。已完成眼镜端拍照→DeepSeek HTTP200→左右眼显示回答的实测。相机已释放，SVR 继续运行；不代表全部定位/摄像头组合均已验证。

图像接口依据 [DeepSeek 官方视觉文档](https://api-docs.deepseek.com/guides/vision/) 接入。

首次构建自动校验并下载官方 Vosk/JNA 与中文模型；模型体积约42MB，不纳入源码 Git。`tools/configure-deepseek.ps1` 提供本机遮罩输入，`tools/provision-deepseek.ps1` 通过授权 USB 通道写入眼镜 Keystore。在工作台启动完成后再使用测试参数 `-TestCloud -TestKind status|answer|vision` 仅接受固定测试，vision 会发送一张实际相机照片给已配置服务。

GPU 实验保留在本地，未集成进候选版本。

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
## 动态 HUD 候选版本 0.3.0（versionCode 15）

- 透明科技蓝动态光环、扫描线、呼吸光和真实麦克风波形；不复制预览里的假电量、延迟或6DoF数据。
- 顶部常驻真实电量/充电、Wi-Fi/联网、手柄与静音状态，所有工作台页面都有返回、主页、回正。手柄返回退出上一层，菜单键回主页并回正。
- 普通问答和 RGB 单帧问答使用兼容接口 SSE 真流式输出；上下发光箭头连续滚动，软件和网络列表按行滚动，取消分页按钮。
- 设置→声音与音量：调整系统媒体音量与静音。默认静音；加减不自动解除静音，只有手动开关才开启声音。浏览器与 Unity 跟随此设置，未加入语音合成。
- 透明键盘临时隐藏下层绘制，返回先关键盘；AI等待页返回取消请求，旧请求不覆盖新页面。

实机结果持续记入执行记录。浏览器网站和其他 Android 应用沿用其原有内容/界面；浏览器 CPU 纹理路径的约8FPS限制未改变。

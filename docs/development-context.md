# 开发上下文

记录日期：2026-11-09。以下是当前证据与用户要求，不是已完成产品设计。

## 用户要求

- 眼镜独立运行；电脑仅用于开发调试，不作为浏览器渲染主机。
- 浏览器首个测试目标为 B 站；已选择 Firefox 143 系列作为候选。
- 云端 AI 支持普通问答，以及后续利用眼镜摄像头和麦克风。
- AI 接口先兼容多家服务，具体服务及密钥稍后配置。
- 当前测试保持静音；录音输入与扬声器输出分开控制。
- 一切改动前建立备份、版本记录和恢复方法。

## 已验证事实

- ADB 序列号 ff72859b；Android 7.1.2 / API 25，userdebug。
- APQ8098 / msm8998，ARM64 与 ARMv7，约 4 GB RAM / 64 GB 存储。
- 系统内核 4.4.63，安全补丁 2017-04-05。
- 启动属性 flash.locked=0 / orange；尚未进入 Fastboot 核实。
- 未发现常见 Treble / VINTF 支持证据；没有经过验证的兼容升级固件。
- 原厂显示、追踪依赖 SVR / qvrservice 和专有相机库。
- SDK 文档标注 Unity 5.6—2017，暂不支持 Unity 2018；不能推断它支持 Unity 6。
- Unity 6 6000.3.27f1 编辑器以及 Android SDK/NDK/JDK 已存在于 D:/Unity/6000.3.27f1。
- 原厂 ExampleScene 已运行，SVR 日志多数接近 60 FPS，偶有降帧；尚未完成实际佩戴的 6DoF 与双目观感测试。
- 电脑热点已连接；HTTPS 测试返回 200。开启自动校时后系统时间从 1970 年恢复。
- 原厂启动器 com.realmax.launcher 和 com.RMSR.mainclient 检查时已禁用；未重新启用。
- realmax-C157 已配对，尚未证实 GATT 活动连接。原厂设置出现 connectIStarService NoSuchMethodError；不能声称手柄可用。
- 原厂设置日志还出现 com.qualcomm.snapdragonvrservice/.SvrService 未找到；需确认与此设备 qvrservice 的关系。
- Android 声明 microphone，音频策略枚举 Built-In Mic 和 Built-In Back Mic；尚未实际录音，不能据枚举断定物理麦克风数量和拾音质量。
- Camera HAL 暴露两个设备：0（BACK，90 度）、1（BACK，0 度）。相机 0 元数据为 2592x1944，设备 1 为 2560x800；需要实际识别 RGB/追踪用途，避免占用追踪相机。
- 尚未启动录音、拍照或向云端上传音视频。

## 待验证

- Firefox 143.0.4 安装、网页显示、B 站播放、登录与解码能力；尚未安装，不能声称兼容。
- RGB 相机采集和空间追踪是否冲突；麦克风实际可录性。
- Unity 6 与旧 SDK 的移植成本、双目画面和原生浏览器组件共享纹理方案。
- 云端服务协议、流式文本、多模态图片、语音转文字；多家服务通过独立适配器接入，不能默认各家完全兼容。
- 设备时间、CA、TLS 兼容性；不通过关闭证书校验或伪造浏览器身份掩盖问题。
- 不在源代码、Git、日志、APK 内硬编码正式 API 密钥。

## 资料

- Firefox Android 7 支持终止：https://blog.mozilla.org/futurereleases/2025/09/15/raising-the-minimum-android-version-for-firefox/
- GeckoView：https://mozilla.github.io/geckoview/
- B 站 HTML5 帮助：https://www.bilibili.com/blackboard/html5playerhelp.html

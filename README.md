# REALMAX 乾独立应用项目

目标：眼镜脱离电脑，通过 Wi-Fi 使用 B 站浏览器与云端 AI。

当前阶段：硬件与 SDK 调查、备份基线准备；尚未创建 Unity 产品工程。

- 开发事实与待验证项：[开发上下文](docs/development-context.md)
- 备份范围与恢复限制：[备份说明](docs/backup-and-recovery.md)
- 基线清单：`docs/baselines/`
- 备份工具：`tools/backup_baseline.py`

原始资料位于 `sdk/` 和 `Export_apk/`；备份位于 `.backups/`，均不作为 Git 大文件提交。Git 保存代码、文档、校验清单。Git 仓库仅在本地，尚未配置远端。

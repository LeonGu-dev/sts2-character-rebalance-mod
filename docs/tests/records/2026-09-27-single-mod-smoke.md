# 单一 Mod 首次游戏内验证

- 日期：2026-09-27
- 游戏版本：`v0.111.0`（游戏 `release_info.json`）
- 源码提交：`39417be`
- 测试范围：用户确认安装后的单一 `RebalanceMod` 可正常运行。没有逐张卡牌、逐个视觉效果的结果记录；角色详细清单仍需单独勾选。
- 安装核对：游戏 `mods/RebalanceMod/` 中 DLL、JSON、PCK、PDB 与本仓库构建产物哈希一致；安装时原文件已备份在本机 `D:\sts2mod\mod-install-backups\20260927-200838`。
- 日志核对：本次 `godot.log` 只发现一个启用的 `RebalanceMod`，加载了 DLL 与 PCK，并调用 `RebalanceMod.ModEntry` 初始化。

## 后续观察项

- 故障机器人自定义能力缺少专属图标，当前会回退到通用 `power.png`；日志中的相关信息不代表卡牌加载失败。
- 日志有两条 `System.Threading.Tasks.Task` 转 Godot Variant 的异常，现有栈没有指向本 Mod；这类信息在迁移前记录中也出现过。若以后遇到实际卡死或失效，再结合卡牌操作和完整调用栈定位。
- 游戏退出时 Godot 报告资源未释放；本轮用户验证没有报告功能故障。

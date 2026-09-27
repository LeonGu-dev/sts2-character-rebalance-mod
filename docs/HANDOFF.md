# 工作交接

更新日期：2026-09-27

## 当前工程

- 主要开发目录：`D:\GitHub\sts2-character-rebalance-mod`。合并包工程为根目录的 `IroncladRebalance.csproj`，产物名 `RebalanceMod`。
- 静默猎手和故障机器人各有独立工程，目录分别为 `SilentRebalance/`、`DefectRebalance/`。
- 原工作目录 `D:\sts2mod` 保留旧 Git 历史、未提交状态、反编译源码和经验笔记，作为本地参考。
- 游戏路径与 MegaDot 路径由被 Git 忽略的 `Directory.Build.props` 配置；仓库内有示例文件。

## 已验证

- 2026-09-27：在新目录中分别编译合并包、静默猎手和故障机器人工程，均为 0 个错误。合并包有 10 个空引用警告，静默猎手 0 个警告，故障机器人有 4 个空引用警告。
- 2026-09-27：合并包执行完整 `dotnet publish`，临时输出中有 DLL、JSON、PDB 和约 1.27 MB 的 `RebalanceMod.pck`。MegaDot 产生 UID 重复及编辑器设置保存等警告；构建目标对导出错误采用 `WarnAndContinue`，每次正式发布仍需核对 PCK。
- 以上验证没有覆盖游戏 `mods/` 目录，也没有完成游戏内效果测试。迁移前记录主要基于游戏 v0.111.0；当前游戏版本尚未重新核实。

## 下一步

- [ ] 核实当前游戏版本，并在该版本下测试合并包及独立包的关键卡牌。
- [ ] 游戏中确认“冷却剂”临时冰霜球的粉色光晕是否可见。旧记录表明代码曾命中，但视觉效果尚未最终确认；参考本地 `STS2交接记录.md` 第 4、9 节。
- [ ] 视觉确认后，评估是否移除 `DefectRebalance/DefectRebalanceCode/Hooks/CoolantOrbGlowPatch.cs` 的高频诊断日志。
- [ ] 按 `tests/silent.md` 执行静默猎手测试，并记录日期、游戏版本和结果。
- [ ] 发布新版本前复核 `release/defect.md`，避免将 v0.111.0 的历史文案直接用于新游戏版本。

## 继续工作

先读根目录 `README.md` 和本文；涉及具体角色时读对应的 `balance/` 文档。需要机制细节或旧日志时，再查本地 `D:\sts2mod` 的历史记录。修改后在本仓库运行对应的 `build.bat` 或 `publish.bat`，验证后把代码与文档一并提交。

# 工作交接

更新日期：2026-09-27

## 当前结构

- 主要目录：`D:\GitHub\sts2-character-rebalance-mod`。唯一工程 `RebalanceMod.csproj`，唯一清单 `RebalanceMod.json`，唯一入口 `src/ModEntry.cs`。
- 三个现有角色的代码分别在 `src/Ironclad/`、`src/Silent/`、`src/Defect/`。资源在 `RebalanceMod/`，卡牌本地化在 `RebalanceMod/localization/`。
- 保留旧自定义卡牌的命名空间和 ID，方便后续在不破坏现有本地化键的前提下改设计。
- 原工作目录 `D:\sts2mod` 是本地历史参考区。游戏与 MegaDot 路径保存在被 Git 忽略的 `Directory.Build.props`。

## 已验证

- 重构前，三个工程分别构建成功；重构后，唯一的 `RebalanceMod.csproj` 构建成功，0 个错误、10 个原有空引用警告。
- 重构后完成 `dotnet publish`，生成约 1.03 MB 的 `RebalanceMod.pck`，并复制到仓库内临时安装目录。PCK 导出日志包含三个角色的图像和英中文本，没有此前由双份资源引起的 UID 重复警告。
- 另一次临时 Godot PCK 挂载检查以 Windows 访问冲突代码 `-1073741819` 退出；未查到内存不足的证据。此临时检查不是游戏内测试，不再重复运行。
- 已将构建产物安装到游戏 `mods/RebalanceMod/`，并核对 DLL、JSON、PCK、PDB 哈希一致。游戏 `release_info.json` 显示 v0.111.0。
- 用户已确认单一 Mod 在游戏内验证成功；日志显示仅加载一个 `RebalanceMod` 并调用 `RebalanceMod.ModEntry`。范围和观察项见 `tests/2026-09-27-single-mod-smoke.md`。源码搜索也确认只有一个 `ModInitializer`。

## 下一步

- [x] 核实当前游戏版本，安装单一 `RebalanceMod` 并完成初次游戏内验证。
- [ ] 后续设计调整时按 `tests/silent.md` 逐项测试静默猎手；另补铁甲战士与故障机器人的详细回归测试。初次整体验证不等于每个复选项均已验证。
- [ ] 确认“冷却剂”临时冰霜球的粉色光晕是否可见。历史技术记录见本地 `D:\sts2mod\STS2交接记录.md`。
- [ ] 视觉确认后评估是否移除 `src/Defect/Hooks/CoolantOrbGlowPatch.cs` 的诊断日志。
- [ ] 发布新版本前核对 `docs/release/defect.md`，其历史文案针对 v0.111.0。

## 继续工作

先读根目录 `README.md` 和本文；涉及具体角色时读 `docs/balance/` 对应文档。修改代码与本地化后运行根目录 `build.bat` 或 `publish.bat`，并在同一提交中更新设计和测试记录。未来两个角色按 `docs/ADDING_CHARACTER.md` 加入唯一工程，设计内容待需求明确后再定。

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
- 另一次临时 Godot PCK 挂载检查以 Windows 访问冲突代码 `-1073741819` 退出；未查到内存不足的证据。PCK 文件与导出日志正常，但尚未完成游戏内加载验证。暂不重复运行该临时检查。
- 源码搜索确认只有一个 `ModInitializer`。尚未将新版本安装到游戏目录，也没有做重构后的游戏内测试。
- 迁移前机制记录主要基于游戏 v0.111.0；当前游戏版本尚未重新核实。

## 下一步

- [ ] 核实当前游戏版本，安装单一 `RebalanceMod` 并进入游戏检查三个角色的卡牌和图像。
- [ ] 按 `tests/silent.md` 执行静默猎手测试，记录游戏版本、日期和结果；另补铁甲战士与故障机器人的回归测试。
- [ ] 确认“冷却剂”临时冰霜球的粉色光晕是否可见。历史技术记录见本地 `D:\sts2mod\STS2交接记录.md`。
- [ ] 视觉确认后评估是否移除 `src/Defect/Hooks/CoolantOrbGlowPatch.cs` 的诊断日志。
- [ ] 发布新版本前核对 `docs/release/defect.md`，其历史文案针对 v0.111.0。

## 继续工作

先读根目录 `README.md` 和本文；涉及具体角色时读 `docs/balance/` 对应文档。修改代码与本地化后运行根目录 `build.bat` 或 `publish.bat`，并在同一提交中更新设计和测试记录。未来两个角色也加入这个唯一工程，设计内容待需求明确后再定。

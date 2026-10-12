# 工作交接

更新日期：2026-10-11

## 当前结构

- 主要目录：`D:\GitHub\sts2-character-rebalance-mod`。唯一工程 `RebalanceMod.csproj`，唯一清单 `RebalanceMod.json`，唯一入口 `src/ModEntry.cs`。
- 四个角色的代码分别在 `src/Ironclad/`、`src/Silent/`、`src/Defect/`、`src/Regent/`。资源在 `RebalanceMod/`，卡牌本地化在 `RebalanceMod/localization/`。
- 保留旧自定义卡牌的命名空间和 ID，方便后续在不破坏现有本地化键的前提下改设计。
- 原工作目录 `D:\sts2mod` 是本地历史参考区。游戏与 MegaDot 路径保存在被 Git 忽略的 `Directory.Build.props`。

## 已验证

- 2026-10-11 总览卡牌详情修正：最新日志明确指向 `ConquerorReplayPatch` 对 canonical 君王之剑读取 Owner，影响大爆炸、追踪之刃、剑圣等关联提示。已在 Owner 读取前增加 `IsMutable` 检查，Release build 通过（0 错误、10 条原有警告），5 项模板数据回归检查通过。新版 DLL/PDB 已安装；备份位于 `.build-mods/install-backups/20261011-card-inspect-235426/`。按 `tests/regent.md` 的 R09-UI1～UI3 重启复测总览显示和战斗重放。
- 2026-10-11 创世纪描述图标修正：中英文卡面及状态的普通／智能描述统一使用游戏原生 `{singleStarIcon}` 蓝星图标。Release build 通过（0 错误、10 条原有警告）；重新导出 PCK，四份 cards/powers JSON 的包内字节与源码一致。已更新仓库暂存 PCK 和游戏安装 PCK，SHA-256 一致；旧 PCK 备份于 `.build-mods/install-backups/20261011-genesis-icons-234133/`。卡面显示待重启游戏确认。
- 2026-10-11 用户确认当前版本验证通过，并准备自行使用游戏上传工具发布到创意工坊。已更新 `RebalanceMod.json` 简短描述及 `docs/release/steam-workshop-description.txt`、`steam-workshop-description-en.txt`：四角色简介与全部 21 项储君调整，玩家文案仅保留最终费用、数值、稀有度和效果，省略开发记录、旧效果对比及特殊结算。此次需求已收窄为准备描述文件。
- 2026-10-11 葬送存档成长修正：用户报告返回标题页后回归 1 蓝星，当前存档及备份中的葬送缺少成长记录。改为在 `CardModel.ToSerializable`／`FromSerializable` 直接写入和恢复 `IncreasedDamage` 成长字段，保持巨镰式牌组原卡持久成长与复制品独立成长。Release build 成功（0 错误、10 条原有警告），16 项本地 JSON 往返数据检查通过；新版 DLL/PDB 已安装且 SHA-256 与构建一致，备份位于 `.build-mods/install-backups/20261011-devastate-save-195308/`。尚待按 `tests/regent.md` 的 R21-S1～S4 重启游戏复测；旧存档未记录的成长无法从中恢复。
- 2026-10-11 根据游戏内反馈修正：征服者回合末移除全部自身层数；强化熔炉在首回合抽牌前置于抽牌堆底，随后按原自动出牌阶段打出；葬送采用狂宴／狩猎的攻击前斩杀许可，爪牙不成长。Release build 成功（0 错误、10 条原有警告）；新 DLL/PDB 已安装到游戏目录并通过 SHA-256 一致性核对，旧文件备份于 `.build-mods/install-backups/20261011-regent-fixes-192055/`。此次只有 C# 和文档变化，沿用已安装 PCK。三项修正版待按 `tests/regent.md` 的 R09-F1～R21-F3 重启复测。
- 2026-10-11 排查储君调整未加载：游戏安装目录仍是 2026-09-27 21:09 的旧 DLL（75,264 字节），含储君补丁的 DLL（101,888 字节）此前仅位于仓库 `.build-mods/RebalanceMod/`。已重新执行 Release publish，将 DLL、JSON、PCK、PDB 安装到游戏 `mods/RebalanceMod/`，四个文件与暂存产物 SHA-256 一致；确认 DLL 含储君补丁命名空间、PCK 中英中 cards/powers JSON 与源码逐字一致。安装前旧文件已备份到 `.build-mods/install-backups/20261011-184016/`。游戏版本仍为 v0.111.0；本次尚待重启游戏验证。
- 本次受限环境中的 MegaDot 弹出 `.NET assemblies not found` 并未产出 PCK；`C:/tools/megadot/GodotSharp/Api/Debug` 文件实际存在，切换正常权限后 export-pack 成功。导出时另有编辑器加载项目程序集缺少 `sts2` 的异常，但退出码为 0，资源已打包并独立核对；此结果不代表游戏内补丁初始化或结算已验证。
- 重构前，三个工程分别构建成功；重构后，唯一的 `RebalanceMod.csproj` 构建成功，0 个错误、10 个原有空引用警告。
- 重构后完成 `dotnet publish`，生成约 1.03 MB 的 `RebalanceMod.pck`，并复制到仓库内临时安装目录。PCK 导出日志包含三个角色的图像和英中文本，没有此前由双份资源引起的 UID 重复警告。
- 另一次临时 Godot PCK 挂载检查以 Windows 访问冲突代码 `-1073741819` 退出；未查到内存不足的证据。此临时检查不是游戏内测试，不再重复运行。
- 已将构建产物安装到游戏 `mods/RebalanceMod/`，并核对 DLL、JSON、PCK、PDB 哈希一致。游戏 `release_info.json` 显示 v0.111.0。
- 用户已确认单一 Mod 在游戏内验证成功；日志显示仅加载一个 `RebalanceMod` 并调用 `RebalanceMod.ModEntry`。范围和观察项见 `tests/2026-09-27-single-mod-smoke.md`。源码搜索也确认只有一个 `ModInitializer`。
- 已修正“谋划专家”只对每回合第一张非奇巧技能牌生效的筛选逻辑。`build.bat` 编译成功（0 个错误、10 个原有警告），新 DLL 已安装到游戏 `mods/RebalanceMod/` 且与构建产物哈希一致；此修正尚待游戏内逐项验证。

## 下一步

- [x] 储君当前版本已获用户验证通过，准备工坊发布；后续按 `tests/regent.md` 回归跨回合、复制与存档联动。
- [x] 核实当前游戏版本，安装单一 `RebalanceMod` 并完成初次游戏内验证。
- [ ] 后续设计调整时按 `tests/silent.md` 逐项测试静默猎手；另补铁甲战士与故障机器人的详细回归测试。初次整体验证不等于每个复选项均已验证。
- [ ] 按 `tests/silent.md` 的“谋划专家”条目验证：攻击牌、能力牌和已有奇巧的技能牌不消耗触发机会，第一张非奇巧技能牌获得奇巧，后续符合条件的技能牌当回合不再触发。
- [ ] 确认“冷却剂”临时冰霜球的粉色光晕是否可见。历史技术记录见本地 `D:\sts2mod\STS2交接记录.md`。
- [ ] 视觉确认后评估是否移除 `src/Defect/Hooks/CoolantOrbGlowPatch.cs` 的诊断日志。
- [ ] 发布新版本前核对 `docs/release/defect.md`，其历史文案针对 v0.111.0。

## 继续工作

先读根目录 `README.md` 和本文；涉及具体角色时读 `docs/balance/` 对应文档。修改代码与本地化后运行根目录 `build.bat` 或 `publish.bat`，并在同一提交中更新设计和测试记录。未来两个角色按 `docs/ADDING_CHARACTER.md` 加入唯一工程，设计内容待需求明确后再定。

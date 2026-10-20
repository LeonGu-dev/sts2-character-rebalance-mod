# 角色平衡 | Character Rebalance

Slay the Spire 2 的角色平衡 Mod：调整**铁甲战士、静默猎手、故障机器人、储君和亡灵契约师**，新增 **硬撑、全神贯注、递归、散热片** 四张卡。卡牌文本支持简体中文与英文，全部内容包含在一个 `RebalanceMod` 中。

Rebalances Ironclad, Silent, Defect, Regent, and Necrobinder in one mod, with four additional cards and English / Simplified Chinese card text.

## 本次更新：亡灵契约师（2026-10-20）

围绕**奥斯提死亡与复活、灵魂和灾厄**调整 20 张牌：

- **灵魂**：0 费、虚无、消耗；打出本身无效果，被消耗时抽 2/3 张牌。回合末虚无消耗产生的抽牌在弃牌后结算，抽到的新牌留到下回合。
- **死亡与复活**：亡魂牵引在奥斯提死亡时抽牌，苦难造成全体伤害，起床气在复活时生成扫荡凝视；首次召唤不算复活。
- **卡牌重做**：骨舞、切断、献祭仪式、化我血骨、灵魂诅咒、女妖之嚎等重新设计，配合召唤与灾厄形成新的联动。
- **原版交互**：抽牌沿用游戏命令及禁止抽牌提示；女妖之嚎的禁召唤属于负面状态，可以被人工制品抵消。

本版已由作者完成基本游戏内测试并确认可先发布；多人、其他 Mod 组合与极端边界情况不作为全部通过的保证。详见[本次更新说明](docs/release/2026-10-20-necrobinder.md)。

Necrobinder update: 20 card changes centered on Osty's death and revival, Souls, and Doom. Basic in-game testing is complete; multiplayer and third-party mod combinations are not fully certified.

## 查看全部改动

- **[完整逐卡对照（83 项）](docs/release/card-changes-zh.md)**：原版与 Mod 对比，包含费用、数值、稀有度、升级及重做机制。
- [中文工坊说明](docs/release/workshop/description-zh.txt) · [English Workshop description](docs/release/workshop/description-en.txt)
- [源码](src/) · [设计与测试文档](docs/README.md)

适用游戏版本：**v0.111.0**。游戏更新后需复核兼容性。

## 安装

需要 **BaseLib 3.4.5**（工程固定此版本；清单允许更新版本，更新后的兼容性需实际验证）。将同一版本的 `RebalanceMod.dll`、`RebalanceMod.json`、`RebalanceMod.pck` 放入游戏 `mods/RebalanceMod/`，启用 BaseLib 与本 Mod 后重启游戏。

请避免同时启用按角色拆分的旧版 Mod，或修改同一卡牌机制的其他 Mod。源码与文档见 [GitHub 仓库](https://github.com/LeonGu-dev/sts2-character-rebalance-mod)。

更新前退出游戏，并备份存档。不要混用不同构建的 DLL、清单与 PCK；卡牌类型与机制有重做，建议用新开局验证，不保证旧局及其他 Mod 的存档兼容性。

反馈问题时请提供游戏版本、启用的其他 Mod、卡牌是否升级、复现步骤和相关游戏日志；闪退时一并保留转储文件，注意去除个人信息。

## 从源码构建

1. 安装 .NET 9 SDK、Slay the Spire 2、MegaDot / Godot **4.5.1 mono**。
2. 将 `Directory.Build.props.example` 复制成 `Directory.Build.props`，填写 `Sts2Path` 和 `GodotPath`；本机配置已忽略。`Sts2PathDiscovery.props` 也会尝试定位游戏。
3. 完全退出游戏。运行 `build.bat` 编译并安装 DLL、JSON、PDB；资源变化或首次构建时运行 `publish.bat` 导出并安装 PCK。
4. `launch.bat` 通过 Steam 启动游戏。核对 DLL、JSON、PCK 后进行游戏内验证。

`build.bat` 不更新 PCK；构建和发布脚本会写入本机游戏 Mod 目录。游戏程序集由本机安装提供，本仓库不分发。NuGet 版本固定在工程文件中。

## 工程结构

| 路径 | 用途 |
| --- | --- |
| `RebalanceMod.csproj`、`RebalanceMod.json` | 单一工程与 Mod 清单 |
| `src/ModEntry.cs` | 唯一初始化入口，注册 Harmony 补丁 |
| `src/Ironclad/`、`src/Silent/`、`src/Defect/`、`src/Regent/`、`src/Necrobinder/` | 五角色代码 |
| `RebalanceMod/localization/` | 简中与英文文本 |
| `RebalanceMod/` | PCK 使用的资源；第三方素材适用单独的权利说明 |
| `docs/` | 玩家说明、设计记录、测试与发布说明 |

旧自定义卡牌命名空间与 ID 保留以兼容已有本地化及存档。新增角色按 [扩展指南](docs/development/adding-character.md) 加入同一个 Mod。

## 许可与素材

原创代码与文档采用 **[MIT License](LICENSE)**，适用范围见 **[第三方内容声明](THIRD_PARTY_NOTICES.md)**。原版卡图、模板素材及第三方代码不属于本项目的 MIT 授权；图片来源与许可状态见 [素材清单](docs/licenses/asset-provenance.md)。

Original code and documentation are MIT licensed. Third-party artwork and upstream-owned content are excluded; see [third-party notices](THIRD_PARTY_NOTICES.md).

Slay the Spire 与 Slay the Spire 2 及其原版素材属于相应权利人。本项目为非官方社区 Mod，不代表 Mega Crit。Mega Crit 的 [内容政策](https://megacrit.com/content-policy/) 允许符合条件的 Mod，但禁止对 Mod 设置付费墙或以其他方式变现（可接受捐赠）；该政策未直接授予原版美术的开放再许可权。

# 角色平衡 | Character Rebalance

Slay the Spire 2 的角色平衡 Mod：调整**铁甲战士、静默猎手、故障机器人和储君**，新增 **硬撑、全神贯注、递归、散热片** 四张卡。卡牌文本支持简体中文与英文，全部内容包含在一个 `RebalanceMod` 中。

Rebalances Ironclad, Silent, Defect, and Regent in one mod, with four additional cards and English / Simplified Chinese card text.

## 查看全部改动

- **[完整逐卡对照（63 项）](docs/release/card-changes-zh.md)**：原版与 Mod 对比，包含费用、数值、稀有度、升级及重做机制。
- [中文完整说明](docs/release/workshop/steam-workshop-description.txt) · [English description](docs/release/workshop/steam-workshop-description-en.txt)
- [中文工坊简版](docs/release/workshop/steam-workshop-description-zh-short.txt) · [English Workshop short description](docs/release/workshop/steam-workshop-description-en-short.txt)
- [源码](src/) · [设计与测试文档](docs/README.md)

适用游戏版本：**v0.111.0**。游戏更新后需复核兼容性。

## 安装

需要 **BaseLib 3.4.5**（工程固定此版本；清单允许更新版本，更新后的兼容性需实际验证）。将同一版本的 `RebalanceMod.dll`、`RebalanceMod.json`、`RebalanceMod.pck` 放入游戏 `mods/RebalanceMod/`，启用 BaseLib 与本 Mod 后重启游戏。

请避免同时启用按角色拆分的旧版 Mod，或修改同一卡牌机制的其他 Mod。源码与文档见 [GitHub 仓库](https://github.com/LeonGu-dev/sts2-character-rebalance-mod)。

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
| `src/Ironclad/`、`src/Silent/`、`src/Defect/`、`src/Regent/` | 四角色代码 |
| `RebalanceMod/localization/` | 简中与英文文本 |
| `RebalanceMod/` | PCK 使用的资源；第三方素材适用单独的权利说明 |
| `docs/` | 玩家说明、设计记录、测试与发布说明 |

旧自定义卡牌命名空间与 ID 保留以兼容已有本地化及存档。新增角色按 [扩展指南](docs/development/adding-character.md) 加入同一个 Mod。

## 许可与素材

原创代码与文档采用 **[MIT License](LICENSE)**，适用范围见 **[第三方内容声明](THIRD_PARTY_NOTICES.md)**。原版卡图、模板素材及第三方代码不属于本项目的 MIT 授权；图片来源与许可状态见 [素材清单](docs/licenses/asset-provenance.md)。

Original code and documentation are MIT licensed. Third-party artwork and upstream-owned content are excluded; see [third-party notices](THIRD_PARTY_NOTICES.md).

Slay the Spire 与 Slay the Spire 2 及其原版素材属于相应权利人。本项目为非官方社区 Mod，不代表 Mega Crit。Mega Crit 的 [内容政策](https://megacrit.com/content-policy/) 允许符合条件的 Mod，但禁止对 Mod 设置付费墙或以其他方式变现（可接受捐赠）；该政策未直接授予原版美术的开放再许可权。

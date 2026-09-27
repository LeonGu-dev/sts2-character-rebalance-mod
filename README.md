# Slay the Spire 2 character rebalance

![角色平衡 / Rebalance 封面](RebalanceMod/mod_image.png)

一个 `RebalanceMod` 同时调整铁甲战士、静默猎手和故障机器人。仓库只有一个 C# 工程、一个 Mod 清单、一个初始化入口和一个 PCK。后续角色也加入同一个 Mod，不再创建职业专属 DLL。

## 目录

| 路径 | 用途 |
| --- | --- |
| `RebalanceMod.csproj`、`RebalanceMod.json` | 唯一工程与 Mod 清单 |
| `src/ModEntry.cs` | 唯一 `ModInitializer`，一次性注册 Harmony 补丁 |
| `src/Ironclad/`、`src/Silent/`、`src/Defect/` | 按角色组织的卡牌和补丁代码 |
| `RebalanceMod/<角色>/images/` | 随 PCK 发布的卡图、图标及 Godot 导入配置 |
| `RebalanceMod/localization/` | 所有角色共用的英中卡牌文本 |
| `docs/` | 交接、设计、测试与发布记录 |

旧卡牌类的命名空间暂时保留，避免改变 BaseLib 根据命名空间生成的卡牌 ID 及已有本地化键。新增角色时先在 `src/` 与 `RebalanceMod/` 下增加对应目录，并继续使用唯一的工程和清单。

## 本地构建

1. 安装 .NET 9 SDK、Slay the Spire 2、所需 NuGet 包及 MegaDot / Godot **4.5.1 mono**。
2. 将 `Directory.Build.props.example` 复制成 `Directory.Build.props`，填写本机的 `Sts2Path` 和 `GodotPath`。此文件被 Git 忽略；`Sts2PathDiscovery.props` 也会尝试自动定位游戏。
3. 完全退出游戏。在仓库根目录运行 `build.bat`，构建 DLL、JSON、PDB 并安装到 `<游戏目录>/mods/RebalanceMod/`。本地化、图片或其他资源变更后运行 `publish.bat`，它还会重新导出并安装 `RebalanceMod.pck`。
4. `launch.bat` 通过 Steam 启动游戏。发布后核对安装目录中的 DLL、JSON、PCK，并在游戏内测试。单纯 `build` 不会更新 PCK。

NuGet 依赖版本固定在工程文件中。完整发布先导出到被忽略的 `.publish/` 临时目录，确认 PCK 存在后再复制进游戏目录。MegaDot 可能在 PCK 成功生成时仍报告编辑器设置警告，因此发布后应继续检查输出文件。

## 文档与旧工作区

阅读 [`docs/README.md`](docs/README.md) 了解文档更新规则，继续工作先看 [`docs/HANDOFF.md`](docs/HANDOFF.md)。以后扩展到其他角色时按 [`docs/ADDING_CHARACTER.md`](docs/ADDING_CHARACTER.md) 操作。迁移前的 `D:\sts2mod` 保留旧 Git 历史、反编译源码和详细排障经验，供本地及其他项目参考；当前代码以本仓库为准。

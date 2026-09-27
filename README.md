# Slay the Spire 2 character rebalance mod

铁甲战士、静默猎手和故障机器人平衡性调整工程。根目录的 `IroncladRebalance.csproj` 当前构建合并包 `RebalanceMod`；静默猎手和故障机器人也各有独立工程。游戏版本和 BaseLib 变化可能要求重新验证补丁。

## 工程位置

| 用途 | 工程 / 代码 | 游戏中的安装目录 |
| --- | --- | --- |
| 三角色合并包（主要工程） | `IroncladRebalance.csproj`、`MySts2ContentModCode/`、`SilentRebalance/SilentRebalanceCode/`、`DefectRebalance/DefectRebalanceCode/` | `<游戏目录>/mods/RebalanceMod/` |
| 静默猎手独立包 | `SilentRebalance/SilentRebalance.csproj` | `<游戏目录>/mods/SilentRebalance/` |
| 故障机器人独立包 | `DefectRebalance/DefectRebalance.csproj` | `<游戏目录>/mods/DefectRebalance/` |

根目录的 `IroncladRebalance/` 与两个角色工程内的同名资源目录保存卡图、图标和本地化。`RebalanceModLocalization/` 保存合并包使用的本地化。构建时生成的根目录 `RebalanceMod/` 是暂存资源，不属于源码。

## 本地准备与构建

1. 安装 .NET 9 SDK、Slay the Spire 2、项目所需 NuGet 包和 MegaDot / Godot **4.5.1 mono**。
2. 复制 `Directory.Build.props.example` 为 `Directory.Build.props`，填写本机 `Sts2Path` 和 `GodotPath`。此文件仅用于本机，已被 Git 忽略。`Sts2PathDiscovery.props` 可在没有显式路径时尝试发现游戏目录。
3. 完全退出游戏，再从仓库根目录执行 `build.bat`（只改 C# 时）或 `publish.bat`（改了本地化、图片或其他资源时）。脚本调用 `dotnet build/publish IroncladRebalance.csproj -c Release`；工程会将 DLL、清单和 PDB 复制到游戏的 `mods/RebalanceMod/`，完整发布还会通过 MegaDot 导出 `RebalanceMod.pck`。
4. 独立包使用 `SilentRebalance/build.bat`、`SilentRebalance/publish.bat` 或 `DefectRebalance/build.bat`、`DefectRebalance/publish.bat`，分别安装到其同名 mod 目录。
5. `launch.bat` 通过 Steam 启动游戏。进入游戏前确认所测试的安装目录中有对应 DLL、JSON 和 PCK；仅执行 build 时不会重建 PCK。

当前脚本的成功提示包含原作者机器的 `D:\Steam` 路径，实际安装目标由工程里的 `ModsPath` 决定。各工程的 PCK 导出命令设为 `WarnAndContinue`，因此 `publish` 返回成功后仍应核对 PCK 是否实际生成。

## GitHub 与游戏发布

此仓库保存源码和资源。使用 `git add`、`git commit`、`git push` 更新 GitHub；这不会安装或上传游戏 Mod。用于本机游戏的安装由上述 `build.bat` / `publish.bat` 完成。若要向玩家分发，请从游戏 `mods/<ModName>/` 取对应的 DLL、JSON、PCK，并在目标游戏版本上测试。仓库不包含游戏本体、反编译源码、NuGet 缓存或本机构建产物。

## 本地参考资料

迁移前的工作目录 `D:\sts2mod` 保留了原工程历史、未提交状态、`.ref/` 反编译和本地化提取文件、`.opencode/skills/` 经验笔记，以及 `STS2交接记录.md`、`STS2美术修改参考.md`、角色调整清单等文档。这些资料适合本机继续查阅，也可供后续其他 Mod 项目参考；新仓库仅承载当前可构建项目。以后应在本仓库修改代码，避免两个目录同时改同一文件。

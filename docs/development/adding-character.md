# 在同一个 Mod 中增加角色平衡调整

本项目始终发布一个 `RebalanceMod`。新增角色时，沿用唯一的 `RebalanceMod.csproj`、`RebalanceMod.json`、`src/ModEntry.cs` 和游戏安装目录 `mods/RebalanceMod/`。

## 开始前

1. 在 `docs/balance/` 新建该角色的调整清单，逐项区分“计划”“已实现”“游戏内已验证”。先明确卡牌原效果、目标效果和强化前后数值。
2. 核实当前游戏版本、卡牌类与本地化键，以当前安装版本的游戏文件为准。
3. 保留既有自定义卡牌的命名空间和 ID。新卡牌的命名空间会影响 BaseLib 生成的 ID；确定后将该 ID 写入文档和本地化，避免发布后随意改名。

## 加入代码和资源

1. 在 `src/<角色>/` 下放该角色的 `Hooks/`、`Cards/`、`Powers/` 等实际需要的代码。Harmony 补丁由唯一入口的 `PatchAll()` 发现，不要再增加 `ModInitializer`。
2. 图片放在 `RebalanceMod/<角色>/images/`，文本统一编辑 `RebalanceMod/localization/eng/cards.json` 和 `zhs/cards.json`。不再为角色创建单独的本地化目录或第二份 Mod 清单。
3. 如果自定义卡牌或能力需要专属图片路径，在 `src/ModEntry.cs` 增加该角色的资源根路径常量，并复用 `src/AssetPaths.cs`。缺少专属能力图标时会使用通用占位图；发布前决定是否补图。
4. 更新角色调整清单及必要的游戏内测试清单。实现与设计说明放在同一次 Git 提交中。

## 验证和发布

1. 完全退出游戏。只改 C# 时运行根目录 `build.bat`；修改图片、本地化或其他资源时运行 `publish.bat`，并确认新的 PCK 实际生成。
2. 确认游戏 `mods/` 下启用的是唯一的 `RebalanceMod`，避免同时启用旧职业独立包。游戏内分别测试旧角色回归和新角色调整，记录游戏版本、日期与结果。
3. 检查最新游戏日志中的加载信息、补丁异常和资源缺失。构建成功不能代替游戏内验证。
4. 更新 `docs/development/handoff.md`，再提交并推送源码、资源与文档。不要把本机 `Directory.Build.props`、游戏 DLL、反编译文件、构建产物或日志提交到仓库。

# 仓库文件与提交边界

路径均相对于仓库根目录。工程和运行资源按构建需求组织。

## 根目录文件

| 文件 | 用途与放置理由 |
| --- | --- |
| `README.md` | GitHub 项目入口，包含安装、构建和文档导航 |
| `LICENSE`、`THIRD_PARTY_NOTICES.md` | 仓库许可及第三方声明，保持在根目录便于查阅 |
| `RebalanceMod.json` | 游戏读取的 Mod 清单，不移入文档目录 |
| `RebalanceMod.csproj`、`RebalanceMod.sln` | 唯一构建工程与解决方案 |
| `Directory.Build.props.example`、`Sts2PathDiscovery.props` | 可提交的配置示例与游戏路径发现规则 |
| `Directory.Build.props` | 本机路径配置，已被 Git 忽略 |
| `build.bat`、`publish.bat`、`launch.bat` | 构建、资源导出与启动脚本，根目录是其工作入口 |
| `project.godot`、`export_presets.cfg` | Godot 工程与导出配置，保持工程根目录位置 |
| `.gitignore`、`.gitattributes` | Git 忽略与文本属性规则 |
| `.vscode/` | 已跟踪的编辑器扩展建议和构建任务配置 |

## 文档目录

| 路径 | 内容 |
| --- | --- |
| `docs/README.md` | 全部现行和历史文档的索引 |
| `docs/balance/` | 角色设计、实现记录和自定义卡牌 ID |
| `docs/tests/` | 可重复执行的游戏内测试清单 |
| `docs/tests/records/` | 已执行测试的日期快照 |
| `docs/release/card-changes-zh.md` | 稳定的玩家逐卡对照入口，工坊文案已引用 |
| `docs/release/workshop/` | `description-zh.txt` 与 `description-en.txt` 两份 BBCode 工坊简介 |
| `docs/development/` | 当前交接、角色扩展和仓库维护指南 |
| `docs/licenses/` | 素材来源与许可范围的详细记录 |
| `docs/development/records/` | 开发、发布、校稿与文档维护记录 |

## 运行资源

`RebalanceMod/` 中的图片、`.png.import` 和 `localization/` JSON 都是运行资源。图片导入设置关联 Godot 资源，文本键关联卡牌与能力 ID；它们不作为普通文档搬迁。素材归属见 [第三方声明](../../THIRD_PARTY_NOTICES.md) 和 [素材清单](../licenses/asset-provenance.md)。

旧命名空间、自定义卡牌 ID 和角色资源目录名用于兼容已有文本与存档，整理文件时保留。代码旁的 `.cs.uid` 同样属于 Godot 关联元数据。

## 本地文件

- `.godot/`、`.build-mods/`、`.publish/`、`bin/`、`obj/`：缓存、构建产物及安装备份，已有忽略规则。
- 游戏 DLL、PDB、PCK、本机配置、日志、反编译参考资料：不作为仓库文档提交。
- 发布前清理可再生成的缓存和过期测试工程；需要保留的诊断脚本与安装备份在仓库外归档。`.publish/` 仅保留最终发布文件与压缩包，不提交到 Git。

- `D:\sts2mod`：旧工作区，含独立角色工程、历史资料与未提交改动；继续保留作本地参考。

新增说明文档按用途进入 `docs/`，根目录保留项目入口、许可声明和工程所需文件。

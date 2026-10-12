# 文档索引

本项目将铁甲战士、静默猎手、故障机器人和储君的调整放在同一个 `RebalanceMod` 中。源码与文档已在 GitHub 发布；安装与构建方法见 [项目首页](../README.md)。

## 玩家与发布材料

| 文档 | 用途 |
| --- | --- |
| [完整逐卡对照（63 项）](release/card-changes-zh.md) | 四角色的原版与 Mod 对比，作为当前玩家说明的统一入口 |
| [中文工坊简版](release/workshop/steam-workshop-description-zh-short.txt) · [English Workshop short description](release/workshop/steam-workshop-description-en-short.txt) | 可直接复制的 BBCode，包含仓库与逐卡对照链接 |
| [中文完整文案](release/workshop/steam-workshop-description.txt) · [English full description](release/workshop/steam-workshop-description-en.txt) | 完整发布文案；复制前核对目标平台的长度限制 |

工坊文案使用 BBCode。完整版与简版按发布平台选用，提交前核对平台长度限制。

## 设计与实现

- [铁甲战士](balance/ironclad.md)
- [静默猎手](balance/silent.md)
- [故障机器人](balance/defect.md)
- [储君](balance/regent.md)

设计文档说明卡牌效果、实现方式、自定义卡牌 ID 和特殊结算。玩家查阅全部改动时使用逐卡对照。

## 测试与验证

| 文档 | 用途 |
| --- | --- |
| [静默猎手回归清单](tests/silent.md) | 可重复执行的逐卡测试 |
| [储君回归清单](tests/regent.md) | 21 项改动及关键联动的回归步骤 |
| [储君验收记录](tests/records/2026-10-12-regent.md) | 游戏内验收结果及修复验证记录 |
| [2026-09-27 单一 Mod 验证记录](tests/records/2026-09-27-single-mod-smoke.md) | 保留当次版本、安装与日志核对结果 |

适用游戏版本为 **v0.111.0**。验收结果和测试范围见测试记录，回归清单用于后续改动与游戏更新。

## 开发与维护

- [当前工作交接](development/handoff.md)：当前结构、最近确认与仍需处理的事项。
- [增加角色指南](development/adding-character.md)：在唯一工程中加入角色的步骤。
- [仓库文件说明](development/repository-layout.md)：非代码文件的用途、位置与提交边界。
- [储君开发记录](development/regent-worklog.md)：源码分析、设计确认与修复经过。
- [说明文档维护记录](development/documentation-worklog.md)：校稿依据、文档整理与发布决策。

## 许可与素材

- [MIT 许可](../LICENSE)：原创代码与文档的许可。
- [第三方内容声明](../THIRD_PARTY_NOTICES.md)：第三方代码、图片等内容的适用范围。
- [素材来源清单](licenses/asset-provenance.md)：素材来源与许可状态。

## 历史归档

归档文件只保留当时的状态、草稿或操作证据；其中的待办和发布判断不覆盖当前工作交接。

- [截至 2026-10-11 的开发日志](archive/2026-10-11-development-log.md)
- [2026-10-12 GitHub 发布检查快照](archive/2026-10-12-publishing-audit.md)
- [历史三角色页面草稿](archive/three-character-page-draft.md)
- [历史故障机器人独立包说明](archive/defect-release.md)

## 维护规则

1. 调整卡牌时，同一提交更新代码、本地化、角色设计记录、逐卡对照及受影响的工坊文案；未完成的想法标为“计划”。
2. 游戏内测试记录版本、日期、测试范围和实际结果。编译、数据检查与打包检查分别记录，不替代游戏内验证。
3. 工作交接只写当前状态和待办；已完成的长日志按日期归档，历史快照不改写成新的验证结论。
4. 新文档放入对应用途目录，并更新本索引。移动文件时修正 Markdown 相对链接、文内路径和发布文案中的 URL；已有外部引用的公开路径尽量保持稳定。
5. 用户确认、校稿过程、排障日志、历史冲突和发布决策只写入工作交接、开发记录、测试记录或历史归档。玩家说明和技术指南只保留当前内容与适用条件。本机资料的存放位置见工作交接。

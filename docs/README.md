# 项目文档管理

此目录跟随代码一起提交到 GitHub。各文档的用途如下：

| 文档 | 记录内容 | 何时更新 |
| --- | --- | --- |
| [`HANDOFF.md`](HANDOFF.md) | 当前可继续工作的状态、已验证事项、下一步任务 | 每次结束一轮开发或排障后 |
| [`balance/ironclad.md`](balance/ironclad.md)、[`balance/silent.md`](balance/silent.md)、[`balance/defect.md`](balance/defect.md)、[`balance/regent.md`](balance/regent.md) | 各角色卡牌的设计与实现清单 | 每次调整卡牌时，与代码同一次提交 |
| [`tests/silent.md`](tests/silent.md) | 可重复执行的游戏内测试项 | 测试后记录游戏版本、日期和结果 |
| [`tests/regent.md`](tests/regent.md) | 储君 21 张卡及关键联动的游戏内验收清单 | 测试后记录游戏版本、日期和结果 |
| [`tests/2026-09-27-single-mod-smoke.md`](tests/2026-09-27-single-mod-smoke.md) | 单一 Mod 首次游戏内验证记录 | 保留为本次验证快照 |
| [`release/defect.md`](release/defect.md) | 给玩家看的历史版本说明 | 准备对应版本发布时 |
| [`release/mod-page-draft.md`](release/mod-page-draft.md) | 单一 Mod 的玩家说明页草稿与发布前校稿项 | 卡牌改动或准备发布时 |
| [`release/steam-workshop-description.txt`](release/steam-workshop-description.txt)、[`release/steam-workshop-description-en.txt`](release/steam-workshop-description-en.txt) | 可粘贴到创意工坊的中英文 BBCode 说明，包含四个角色 | 准备工坊发布时 |
| [`ADDING_CHARACTER.md`](ADDING_CHARACTER.md) | 将新角色加入唯一 Mod 的步骤 | 工程结构或发布流程变化时 |

## 更新规则

1. 修改卡牌时，在对应角色汇总里写清卡牌名、原效果、目标效果和实现状态；同一次提交更新 `src/<角色>/`、`RebalanceMod/localization/` 和文档。未完成的想法标为“计划”，不要写成已实现。新增角色仍沿用唯一工程和清单。
2. 构建成功只证明工程可编译。游戏内效果、视觉效果和 PCK 内容需单独验证，并记录游戏版本。旧版本结论保留日期，不自动沿用到新版本。
3. `HANDOFF.md` 保持简短，只写当前状态和仍需处理的事项。排障过程中的长日志、反编译行号和本机路径留在本地参考区。
4. 本机参考区为 `D:\sts2mod`，其中的 `.ref/`、旧版 `STS2交接记录.md`、`STS2美术修改参考.md`、`故障机器人核心机制参考.md` 和 `.opencode/skills/` 继续保留。它们是历史资料；涉及游戏机制时以当前游戏文件和代码复核。

迁移时保留了原文档的主体内容，并修正了已发现的“冰之长枪”描述冲突。其余卡牌说明尚未逐条做游戏内复测。

## 玩家页面与公开准备

- [完整逐卡对照（63 项）](release/card-changes-zh.md)
- [素材来源与许可待确认项](release/asset-provenance.md)
- [GitHub 发布检查](release/publishing-audit-zh.md)

当前工坊简版为 steam-workshop-description-zh-short.txt 与 steam-workshop-description-en-short.txt；中文完整版超过此前报告的 8,000 UTF-8 字节限制。2026-10-12 已确定原创代码与文档采用 MIT，保留现有资源；第三方内容不属于 MIT 授权，详见根目录 THIRD_PARTY_NOTICES.md。

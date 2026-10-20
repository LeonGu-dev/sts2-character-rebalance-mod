# 文档索引

安装与构建方法见 [项目首页](../README.md)。适用游戏版本为 **v0.111.0**。

## 玩家说明与发布文案

- [2026-10-20 亡灵契约师更新说明](release/2026-10-20-necrobinder.md)：玩法重点、安装、基本测试与兼容性边界。
- [完整逐卡对照（83 项）](release/card-changes-zh.md)：五角色的原版与 Mod 对比。
- [中文工坊说明](release/workshop/description-zh.txt) · [English Workshop description](release/workshop/description-en.txt)：可直接复制的 BBCode 简介，包含项目与详细改动链接。

## 角色设计与实现

- [铁甲战士](balance/ironclad.md)
- [静默猎手](balance/silent.md)
- [故障机器人](balance/defect.md)
- [储君](balance/regent.md)
- [亡灵契约师（骨妹）](balance/necrobinder.md)

角色文档说明卡牌效果、实现方式、自定义卡牌 ID 和特殊结算。

## 回归清单与测试记录

- [静默猎手回归清单](tests/silent.md)
- [储君回归清单](tests/regent.md)
- [亡灵契约师回归清单](tests/necrobinder.md)
- [亡灵契约师手工测试用例](tests/necrobinder-manual.md)：41 个用例，包含前置、操作、预期和优先级。
- [亡灵契约师本轮本地实现检查](tests/records/2026-10-20-necrobinder-local.md)
- [亡灵契约师手工测试与 M06 闪退处理](tests/records/2026-10-20-necrobinder-manual.md)
- [储君验收与修复验证记录](tests/records/2026-10-12-regent.md)
- [单一 Mod 首次验证记录](tests/records/2026-09-27-single-mod-smoke.md)

## 工作文档

开发目录顶层放当前交接和指南，`development/records/` 统一存放工作历史。

- [当前工作交接](development/handoff.md)
- [增加角色指南](development/adding-character.md)
- [仓库文件说明](development/repository-layout.md)
- [储君开发记录](development/records/regent.md)
- [开发与安装日志](development/records/2026-10-11-development.md)
- [GitHub 发布检查记录](development/records/2026-10-12-publishing.md)
- [2026-10-20 亡灵契约师发布检查](development/records/2026-10-20-publishing.md)
- [文档整理与校稿记录](development/records/2026-10-12-documentation.md)

## 许可与素材

- [MIT 许可](../LICENSE)
- [第三方内容声明](../THIRD_PARTY_NOTICES.md)
- [素材来源清单](licenses/asset-provenance.md)

## 维护规则

1. 卡牌调整时同步更新代码、本地化、角色说明、逐卡对照和中英文工坊文案。
2. 玩家说明与技术指南保留当前内容；用户确认、校稿过程、排障日志和发布决策写入工作记录或测试记录。
3. 工作交接只保留当前状态与待办，历史过程写入 `development/records/`；测试结果写入 `tests/records/`，注明版本、日期和实际范围。
4. 移动文件时修正相对链接和文内路径；逐卡对照的公开路径保持稳定。
5. 被现行材料替代的重复草稿删除，旧版本通过 Git 历史查阅。代码、工程配置和运行资源按构建需要保留。

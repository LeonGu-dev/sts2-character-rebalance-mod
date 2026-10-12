# 素材来源与发布范围

检查日期：2026-10-11；发布决策更新：2026-10-12。作者已选择 MIT 授权原创代码与文档，并保留现有资源公开项目。MIT 不涵盖第三方素材；未取得新的美术授权，本清单保留已知证据缺口，不将下载来源当作再分发授权。许可范围见 [第三方内容声明](../../THIRD_PARTY_NOTICES.md)。

| 文件组 | 可查证来源 | 当前结论 |
| --- | --- | --- |
| `SilentRebalance/images/card_portraits/{,big/}concentrate.png` | `docs/balance/silent.md` 明确记载 STS1 Wiki `Green-Concentrate-Art.png` | 原版游戏美术；下载来源明确，具体再分发授权未确认 |
| `IroncladRebalance/images/card_portraits/{,big/}power_through.png` | 本地历史制作指南记录 `https://slaythespire.wiki.gg/images/Red-PowerThrough-Art.png` | 有来源记录，尚未进行原图哈希或视觉同源复核；不声明自主版权 |
| `DefectRebalance/images/card_portraits/{,big/}{recursion,heat_sinks,recycle}.png` | 历史记录仅说明采用 Wiki 卡图 | 尚缺每张素材的精确下载页与授权证据；不得据此列为原创 |
| 三角色的 `card.png`、`power.png`、`relic.png`、`relic_outline.png` 及 big 版本 | 各组跨角色 SHA-256 相同，疑似工程模板占位资源 | 来源与许可未确认；还存在缺图 fallback 使用，移除前需配套替代 |
| 三个旧角色目录的 `mod_image.png` | 三份 SHA-256 相同 | 来源未确认，需检查模板或制作记录 |
| `RebalanceMod/mod_image.png` | 提交 `204338b` 记录为双语封面 | 制作方式、字体及第三方图形的许可待作者确认 |
| `.png.import` | Godot 导入配置 | 不是原图授权文件；如排除图像，应同时处理对应 import |
| 巩固、储君原版卡图及征服者图标 | 代码引用玩家已安装的游戏资源；没有新增对应复制图像 | 属于游戏资源；运行时引用与把原图复制到公开仓库应区别处理 |

源代码存在与 `Alchyr/ModTemplate-StS2` 模板相似的基础卡牌类注释和结构。2026-10-11 GitHub 元数据没有识别出模板许可，根目录也未发现 LICENSE。应确认实际上游版本、允许使用的依据及需要保留的通知后，才将自己拥有权利的部分授予所选代码许可。BaseLib、Harmony、Godot 和游戏本体各自权利不因本项目许可而改变。

## 素材发布决策与历史讨论

2026-10-12 作者确认采用 MIT 并进入推送、公开流程；沿用现有资源，不重写历史。以下为审查时讨论的替代方式，保留作为后续素材治理参考，不代表尚待选择。

1. **公开源码与文档，GitHub 排除未确认素材**：保留当前本机资源，准备素材恢复说明或替换方案；公开源码包在恢复所需资源前不能承诺直接构建完整 PCK。Steam 现有包的素材分发仍需单独确认。
2. **替换成自己制作或取得明确许可的素材**：来源和许可补全后再公开资源，使 GitHub 可以完整构建。
3. **保留现有素材（本次选择）**：补充来源与第三方通知；当前未确认的授权依据如实标注，不能仅以来自 Wiki 证明可以重新授权。

图片已存在于旧提交中。仅新增 `.gitignore` 或删除当前版本文件不能把它们从历史中移除。如未来选择排除素材，需要另行审阅历史清理方案或无这些素材的新公开历史；本次保留历史。

## 官方政策边界

[Mega Crit Content Policy](https://megacrit.com/content-policy/) 支持符合条件的 Mod、要求遵守平台条款，不得暗示官方关联，并限制 Mod 变现。该页面没有明示允许将原版美术单独开放再许可。因此本项目 MIT 明确限定适用范围，并保留第三方素材归属说明。

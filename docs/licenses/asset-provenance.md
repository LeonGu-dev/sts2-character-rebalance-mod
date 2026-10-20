# 素材来源与发布范围

本清单记录仓库素材来源及许可状态。MIT 仅适用于作者有权授权的原创代码与文档，不涵盖第三方素材。下载来源不等于再分发授权。许可范围见 [第三方内容声明](../../THIRD_PARTY_NOTICES.md)。

| 文件组 | 可查证来源 | 当前结论 |
| --- | --- | --- |
| `SilentRebalance/images/card_portraits/{,big/}concentrate.png` | STS1 Wiki `Green-Concentrate-Art.png` | 原版游戏美术；下载来源明确，具体再分发授权未确认 |
| `IroncladRebalance/images/card_portraits/{,big/}power_through.png` | 来源地址：`https://slaythespire.wiki.gg/images/Red-PowerThrough-Art.png` | 有来源记录，尚未进行原图哈希或视觉同源复核；不声明自主版权 |
| `DefectRebalance/images/card_portraits/{,big/}{recursion,heat_sinks,recycle}.png` | Wiki 卡图，精确来源页未登记 | 尚缺每张素材的精确下载页与授权证据；不得据此列为原创 |
| 三角色的 `card.png`、`power.png`、`relic.png`、`relic_outline.png` 及 big 版本 | 各组跨角色 SHA-256 相同，疑似工程模板占位资源 | 来源与许可未确认；还存在缺图 fallback 使用，移除前需配套替代 |
| 三个旧角色目录的 `mod_image.png` | 三份 SHA-256 相同 | 来源未确认，需检查模板或制作记录 |
| `RebalanceMod/mod_image.png` | 项目双语封面 | 制作方式、字体及第三方图形的许可待作者确认 |
| `.png.import` | Godot 导入配置 | 不是原图授权文件；如排除图像，应同时处理对应 import |
| 巩固、储君原版卡图及征服者图标 | 代码引用玩家已安装的游戏资源；没有新增对应复制图像 | 属于游戏资源；运行时引用与把原图复制到公开仓库应区别处理 |
| `Necrobinder/images/powers/{death_draw,death_blast,price_of_power}.svg` | 2026-10-20 为本 Mod 编写的 SVG 几何图形，未复制外部图片 | 原创矢量图标，适用仓库 MIT 许可 |

## 官方政策边界

[Mega Crit Content Policy](https://megacrit.com/content-policy/) 支持符合条件的 Mod、要求遵守平台条款，不得暗示官方关联，并限制 Mod 变现。该页面没有明示允许将原版美术单独开放再许可。因此本项目 MIT 明确限定适用范围，并保留第三方素材归属说明。

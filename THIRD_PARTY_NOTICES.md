# Third-party notices / 第三方内容声明

## License scope

The root MIT license applies to original code and documentation that LeonGu-dev has the right to license. It does not relicense third-party code, game artwork, template artwork, fonts, or other third-party material. Existing third-party notices must be preserved.

根目录 MIT 许可仅适用于 LeonGu-dev 有权授权的原创代码与文档。第三方代码、游戏美术、模板图片、字体及其他第三方素材不因置于本仓库而改为 MIT；原有通知应予保留。

## Game artwork

Slay the Spire and Slay the Spire 2 and their original artwork belong to their respective rights holders, including Mega Crit. This is an unofficial community mod, with no claim of endorsement or affiliation. The author's distribution is free, primarily through Steam Workshop.

游戏及原版美术的权利归其相应权利人所有。本项目是非官方社区 Mod，不代表 Mega Crit；作者免费分发，主要发布途径为 Steam 创意工坊。

| Repository files under `RebalanceMod/` | Source record | License status |
| --- | --- | --- |
| `SilentRebalance/images/card_portraits/concentrate.png` and `big/concentrate.png` | Slay the Spire Wiki, `Green-Concentrate-Art.png`; [file page](https://slaythespire.wiki.gg/wiki/File:Green-Concentrate-Art.png) | Original game art; specific redistribution permission not independently confirmed |
| `IroncladRebalance/images/card_portraits/power_through.png` and `big/power_through.png` | Recorded source: [Wiki image](https://slaythespire.wiki.gg/images/Red-PowerThrough-Art.png) | Source record exists; image identity and specific redistribution permission not independently confirmed |
| `DefectRebalance/images/card_portraits/{recursion,heat_sinks,recycle}.png` and matching `big/` files | Wiki-sourced card art; exact source pages not recorded | Exact source pages and permission not yet confirmed |
| Character resource folders' `card.png`, `power.png`, `relic.png`, `relic_outline.png` and large variants | Matching files across character folders; likely template placeholders | Exact provenance and license not yet confirmed |
| Character folders' legacy `mod_image.png` | Matching legacy cover files | Exact provenance and license not yet confirmed |
| Root `mod_image.png` | Project's bilingual cover | Font and any third-party graphic provenance not fully recorded; excluded from the code MIT grant |

Wiki hosting is not, by itself, a grant to relicense underlying game artwork. No claim is made that these images are CC-licensed, MIT-licensed, or free for commercial reuse. Inclusion in this repository is not a substitute for permission from the relevant rights holder. Detailed records: [asset provenance](docs/licenses/asset-provenance.md).

Wiki 下载来源本身不构成原版美术的再许可授权。本仓库不宣称上述图片采用 CC、MIT 或可自由商用；素材声明也不代替权利人授权。素材的来源和许可状态见素材清单。

Entrench and Regent portraits and some status icons are referenced from a user's installed game rather than copied into this repository. Game assemblies, soundtracks and a decompiled game source tree are not distributed here.

## Dependencies and template-derived portions

- [BaseLib](https://github.com/Alchyr/BaseLib-StS2): dependency referenced as `Alchyr.Sts2.BaseLib` 3.4.5.
- [Harmony](https://github.com/pardeike/Harmony): runtime patching dependency supplied by the game installation.
- [Godot](https://godotengine.org/) / MegaDot: build and resource-export tooling.
- [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2): some base-card comments and structure resemble the mod template. the project's own MIT grant does not purport to grant rights to upstream-owned portions or artwork.

Each dependency and upstream-owned portion remains subject to its own applicable terms. The project MIT grant does not authorize use of game content or override [Mega Crit's content policy](https://megacrit.com/content-policy/).

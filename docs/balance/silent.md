# 静默猎手 Mod 修改清单

记录来源：迁移前的修改清单。表中的“当前游戏版本”指原记录检查时的版本；升级游戏后需重新核对。

## 本轮修改

| 卡牌 | ID | 修改内容 | 实现方式 |
|---|---|---|---|
| 计算下注 | `CALCULATED_GAMBLE` | 升级后移除消耗，不再获得保留 | Harmony 升级补丁 |
| 腐蚀波 | `CORROSIVE_WAVE` | 每次抽牌仅随机一名敌人获得中毒 | 替换 `CorrosiveWavePower.AfterCardDrawn` |
| 独门技术 | `EXPERTISE` | v0.111.0 原版已是蓝卡（Uncommon），现有稀有度补丁不产生实际改动；不计入发布清单 | `CardModel.Rarity` 基类 getter 分派 |
| 全神贯注 | `SILENTREBALANCE-CONCENTRATE` | 新增 STS1 回归卡：0费，弃置3/2张牌，获得2点能量 | 新增静默卡牌类 |
| 翻越撑击 | `FLICK_FLACK` | 当前游戏版本已为7/9，无需额外补丁 | 已核对游戏源码 |
| 谋划专家 | `MASTER_PLANNER` | 费用改为1/0；每回合打出的第一张非奇巧技能牌获得奇巧。攻击牌、能力牌和已有奇巧的技能牌不占用触发机会 | 费用补丁与能力触发补丁 |
| 触不可及 | `UNTOUCHABLE` | 格挡由原版 6/9 提高为 7/10 | `CanonicalVars` 基值补丁；升级仍增加 3 点格挡 |
| 幽魂形态 | `WRAITH_FORM` | 获得固有、保留、奇巧 | `CardModel.CanonicalKeywords` 基类 getter 分派 |

## 相关文件

- `src/Silent/Cards/Concentrate.cs`
- `src/Silent/Hooks/CardBalancePatches.cs`
- `RebalanceMod/localization/zhs/cards.json`
- `RebalanceMod/localization/eng/cards.json`
- `RebalanceMod/SilentRebalance/images/card_portraits/concentrate.png`
- `RebalanceMod/SilentRebalance/images/card_portraits/big/concentrate.png`

## 卡图来源

- 来源：STS1 Wiki 的 `Green-Concentrate-Art.png`
- 未使用 STS1 游戏 atlas 裁图
- 普通卡图：`250x190`
- 大卡图：`500x380`

## 发布状态

以下为迁移前的历史记录，并非最新游戏安装状态：曾生成独立的 `SilentRebalance.pck`。当前静默猎手代码已并入唯一的 `RebalanceMod`，不再发布独立 DLL 或 PCK；游戏内功能仍待按测试清单复核。

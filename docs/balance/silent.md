# 静默猎手 Mod 修改清单

记录来源：迁移前的修改清单。表中的“当前游戏版本”指原记录检查时的版本；升级游戏后需重新核对。

## 本轮修改

| 卡牌 | ID | 修改内容 | 实现方式 |
|---|---|---|---|
| 计算下注 | `CALCULATED_GAMBLE` | 升级后移除消耗，不再获得保留 | Harmony 升级补丁 |
| 腐蚀波 | `CORROSIVE_WAVE` | 每次抽牌仅随机一名敌人获得中毒 | 替换 `CorrosiveWavePower.AfterCardDrawn` |
| 独门技术 | `EXPERTISE` | 稀有度恢复为蓝卡（Uncommon） | `CardModel.Rarity` 基类 getter 分派 |
| 全神贯注 | `SILENTREBALANCE-CONCENTRATE` | 新增 STS1 回归卡：0费，弃置3/2张牌，获得2点能量 | 新增静默卡牌类 |
| 翻越撑击 | `FLICK_FLACK` | 当前游戏版本已为7/9，无需额外补丁 | 已核对游戏源码 |
| 谋划专家 | `MASTER_PLANNER` | 费用改为1/0；每回合第一张牌获得本场战斗永久奇巧 | 费用补丁与能力触发补丁 |
| 触不可及 | `UNTOUCHABLE` | 当前游戏版本已为7/9，无需额外补丁 | 已核对游戏源码 |
| 幽魂形态 | `WRAITH_FORM` | 获得固有、保留、奇巧 | `CardModel.CanonicalKeywords` 基类 getter 分派 |

## 相关文件

- `SilentRebalance/SilentRebalanceCode/Cards/Concentrate.cs`
- `SilentRebalance/SilentRebalanceCode/Hooks/CardBalancePatches.cs`
- `SilentRebalance/SilentRebalance/localization/zhs/cards.json`
- `SilentRebalance/SilentRebalance/localization/eng/cards.json`
- `SilentRebalance/SilentRebalance/images/card_portraits/concentrate.png`
- `SilentRebalance/SilentRebalance/images/card_portraits/big/concentrate.png`

## 卡图来源

- 来源：STS1 Wiki 的 `Green-Concentrate-Art.png`
- 未使用 STS1 游戏 atlas 裁图
- 普通卡图：`250x190`
- 大卡图：`500x380`

## 发布状态

以下为迁移前的历史记录，并非最新游戏安装状态：曾执行 `SilentRebalance\\publish.bat`，生成 `SilentRebalance.pck`；当时 DLL 编译无错误、无警告，仅包含静默模组自己的初始化器。迁移后独立工程已再次编译成功，游戏内功能仍待按测试清单复核。

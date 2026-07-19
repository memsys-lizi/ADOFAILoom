# 轨道布局与动画

## 1. 四类轨道事件的职责

| 事件 | 职责 |
|---|---|
| PositionTrack | 建立静态布局基线，并默认向后传播 |
| MoveTrack | 在运行时对 floor 范围做位置、旋转、缩放和透明度 Tween |
| RecolorTrack | 改变轨道色彩、样式、发光和持续着色模式 |
| AnimateTrack | 从某 floor 起改变后续轨道的出现/消失策略 |

不要把 PositionTrack 当作 duration=0 的 MoveTrack，也不要用大量 MoveTrack 替代本应由 AnimateTrack 表达的连续出现策略。

## 2. PositionTrack：构图，不是动画

常见用途：

- 将后续路径整体偏移到新的构图区。
- 建立上下分层或左右分栏。
- 调整静态旋转、scale 和 opacity。
- 用 `justThisTile=true` 做单块例外。

样本中常见位置偏移为 `[0,1]`、`[1,0]`、`[0,0.5]`、`[0,-1]`，90% 的轴偏移大致不超过 3 tile。

默认情况下 PositionTrack 会影响后续 floor。每次使用都必须判断传播何时结束、是否需要恢复，不能只看当前 tile。

## 3. MoveTrack：运行时运动

样本中的 duration 中位数约 4 拍，常见为 1–8 拍。

目标语义：

- position 相对每块 floor 的 startPos。
- rotation 相对每块 floor 的 startRot。
- scale 和 opacity 是绝对目标百分比。
- `gapLength=N` 实际每隔 N+1 块处理一次。

### 轨道入场

- 从小范围偏移、scale 0 或 opacity 0 开始。
- 1–4 拍回到正常位置、100 scale、100 opacity。
- OutCubic/OutBack 是最常见的稳定选择。
- 默认只处理即将进入视野的一小段轨道。

### 轨道退场

- 4–8 拍将 opacity 降到 0。
- 同时向画面外小幅偏移或缩小。
- InCubic/InBack 让退场逐渐加速。
- 不得影响玩家当前与马上需要判定的 floor。

### 轨道冲击

- 只选位置、旋转、scale 中一到两个通道。
- 用很短的建立状态和 1–2 拍回弹。
- 避免对整条轨道施加强回弹；小范围更清楚。

## 4. RecolorTrack：段落色彩状态

成熟谱以 Single 和 Glow 为主；Rainbow、Blink、Switch 是强调语言。

- 常规段落使用稳定主色或主辅色。
- 进入新段落时与背景、Bloom、装饰共同换色。
- pulse 默认 None；只有确实需要沿路径流动时使用 Forward/Backward。
- `trackPulseLength` 是 floor 数量。
- `trackColorAnimDuration` 是运行时秒周期，不是拍。
- style 会立即切换，不随 duration 渐变。

如果样式也要切换，最好用 Flash 或遮罩覆盖瞬时变化。

## 5. AnimateTrack：持续出现/消失策略

常用出现：

- Fade：中性、易读。
- Extend：强调路径方向。
- Grow：强调尺度。
- Drop/Rise：明确的上下方向构图。

常用消失：

- Fade：平稳清理。
- Retract：强化路径收束。
- Shrink：强调消散。
- Scatter：风格强、带随机性，只在不要求精确构图时使用。

样本中的 `beatsAhead` 中位数为 3，常用 3–6；慢速整体出现可用 8–20。`beatsBehind` 常用 0–4。这两个参数是拍数，不是 tile 数。

## 6. 轨道与相机协同

轨道和相机应共享一个运动命题：

- 镜头推近时，轨道可轻微放大或从外侧进入。
- 镜头横移时，轨道沿相反方向小幅进入以形成视差。
- 镜头固定展示大图形时，轨道应更稳定。
- 大角度镜头旋转时，轨道不再叠加无关的随机旋转。

在 96 个主谱中，MoveCamera 与 MoveTrack 同时出现于 94 个，说明二者通常应共同设计。

## 7. 防跳变与范围规则

同属性的新 MoveTrack 会完成旧 Tween 后再启动新 Tween。为了避免跳变：

- 同一 floor 范围、同一属性的 Tween 不要无意重叠。
- 批量事件先检查范围交叉。
- 退出动画结束后再由另一事件接管。
- 不要把几十万 tile 的极端 positionOffset 当作正常移出画面方式；那来自少数生成型谱。
- start/end 范围要围绕视觉目的，而不是默认全关卡。

## 8. 轨道自检

- 当前和下一判定轨道是否始终清晰？
- 这是静态布局还是运行时动画，事件类型是否选对？
- PositionTrack 的传播在哪里结束？
- MoveTrack 范围是否过大？
- 入场是否使用 Out*，退场是否使用 In*，并且与运动方向一致？
- RecolorTrack 是否与本段 palette 一致？
- AnimateTrack 的随机类型是否会破坏精确构图？
- 是否存在同属性重叠 Tween？


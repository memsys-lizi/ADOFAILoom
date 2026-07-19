# 组合配方与转场

这些配方是设计骨架，不是要求复制固定数值。floor、angleOffset 和强度由谱师提供的段落与强调点决定。

## 1. 普通镜头击打

目标：给单个强调点增加重量，不改变段落身份。

1. duration=0，以 Player 或 LastPosition 改变 zoom 约 10–25。
2. rotation 保持 0，或短暂偏移约 3–10°。
3. duration=1–2，OutSine/OutCubic 回到镜头基线。
4. 不叠加强滤镜和高 opacity Flash。

适合普通重音和小节内强调。

## 2. 强镜头击打

目标：段落内少数高强度节点。

1. duration=0，zoom 改变约 25–60，可加小位置偏移。
2. duration=2–4，OutCubic/OutBack 回位。
3. 可加前景高光色 Flash，20–40%，1–2 拍淡出。
4. 只有在本段主题已建立时才加入短 Aberration/Fisheye。

不要同时加入强 Shake、轨道大移位和 100% 白闪。

## 3. 白色或高光色击打闪

```text
plane: Foreground
startOpacity: 20–50
endOpacity: 0
duration: 1–4 beats
ease: OutSine / OutCubic / OutQuad
```

颜色优先取本段高光色。纯白用于没有更合适高光色或需要明确曝光感的节点。

## 4. 背景色洗

```text
plane: Background
startOpacity: 10–25
endOpacity: 0
duration: 2–4 beats
color: 当前 palette 主色或辅色
```

用于普通强调、段落颜色呼吸和不遮挡轨道的亮度响应。

## 5. 黑场切镜

目标：安全完成不连续的场景切换。

1. 前景黑色瞬时覆盖。
2. 在黑场内隐藏旧装饰组。
3. 关闭旧滤镜和段落 gimmick。
4. 切背景、轨道色和镜头基线。
5. 显示新装饰组。
6. 0.5–4 拍把黑场淡到 0。

黑场是事务边界。任何一步失败都不应报告整个转场完成。

## 6. 完整段落换景事务

同一 floor、同一逻辑批次中：

1. 用短 Flash 或前景遮罩覆盖切换。
2. 将旧段落临时装饰退场或隐藏。
3. 关闭旧段落专属滤镜、HallOfMirrors、ScreenTile/Scroll。
4. CustomBackground 切换背景。
5. RecolorTrack 建立新 palette。
6. MoveCamera 建立新 relativeTo、position、rotation、zoom 基线。
7. 新段落装饰按 bg → mid → fg 顺序入场。
8. Bloom 在 2–4 拍内过渡到新颜色和基线。
9. Flash 或遮罩完全退场。
10. 记录所有临时状态的后续恢复点。

Workshop 中 80.4% 的背景切换 floor 同时包含 MoveCamera，60.6% 有 Flash，59.3% 有 MoveDecorations。这说明换背景本身不是完整转场。

## 7. 装饰入场

1. AddDecoration 建立稳定 tag 和正确 depth/parallax。
2. 初始 opacity=0，或 scale 接近 0，或位置在构图外。
3. 1–4 拍 OutCubic/OutBack 进入目标状态。
4. 只修改需要改变的通道。
5. 入场结束后进入明确保持状态。

背景用平滑淡入，中景用位移/缩放，前景只做短时遮罩或框架。

## 8. 装饰退场

1. 1–4 拍 opacity 到 0，或沿运动方向移出，或缩小。
2. 使用 InCubic/InBack。
3. 动画完成后保持隐藏。
4. 以后复用时重新建立完整目标状态，不能假设仍在最初位置。

## 9. 轨道入场

方案 A：

- AnimateTrack Fade/Extend。
- beatsAhead 3–6。
- 保持玩家前方有足够可读轨道。

方案 B：

- MoveTrack 从小偏移、scale 0 或 opacity 0 开始。
- 1–4 拍 OutCubic/OutBack 回到标准状态。
- 只覆盖即将出现的小范围。

## 10. 轨道退场

方案 A：

- AnimateTrack Fade/Retract/Shrink。
- beatsBehind 0–4。

方案 B：

- MoveTrack 4–8 拍。
- opacity 到 0，并向画面外小幅偏移。
- InCubic/InBack。

任何方案都不能让当前判定附近的轨道提前消失。

## 11. 滤镜冲击

1. 读取当前滤镜 baseline 和运行时合法范围。
2. duration=0 设置冲击值。
3. duration=1–2，OutQuad/OutCubic 回到 baseline。
4. 如果本段不再使用，回到中性后再禁用。

Aberration、Fisheye 常适合这种模式；Waves、Sharpen、MotionBlur 等必须使用各自语义，不能共享数值模板。

## 12. Bloom 呼吸

1. 保持 enabled=true。
2. 用 2–4 拍在本段低强度和高强度之间过渡。
3. color 只在段落换色时变化，不每拍随机跳色。
4. 段落结束先回到低强度，再禁用。

## 13. 蓄力与释放

蓄力阶段：

- 镜头缓慢推近。
- 前景暗角或背景色洗逐渐增强。
- Shake 使用 In* 从弱到强，但保持可读。
- 轨道保持稳定，避免同时制造高复杂度。

释放点：

- 用切镜或短冲击镜头。
- Flash、轨道动作、滤镜中选一个主效果和一个辅助。
- 立即进入 1–4 拍的回落动画。

## 14. 高潮构建

高潮不是把全部效果设为最大，而是让此前建立的视觉 motif 同时回归：

- 使用已知 palette 的更亮版本。
- 组合之前出现过的图形层。
- 提高镜头和轨道动作频率，但保留稳定基线。
- 把最高强度 Flash/Shake 留给极少数节点。
- 高潮后显著减少活跃层，形成回落。

## 15. 冷却与恢复

1. 回到稳定 Player 镜头。
2. 减少前景和后期。
3. 关闭临时滤镜和屏幕 gimmick。
4. Bloom 回到低基线。
5. 轨道恢复稳定位置、scale 和 opacity。
6. 保留一个主题装饰或颜色，维持作品连续性。


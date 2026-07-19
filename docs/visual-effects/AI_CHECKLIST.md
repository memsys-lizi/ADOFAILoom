# AI 执行与审查清单

## 1. 开始前必须确认

- [ ] 当前处于编辑器且关卡已加载。
- [ ] 已取得精确 current revision。
- [ ] floor 范围由谱师给出。
- [ ] 段落边界、强调点、转场点由谱师给出。
- [ ] 已知每段视觉意图、palette 和强度等级。
- [ ] 已知必须保留、禁止修改和禁止使用的内容。
- [ ] 已读取目标范围及相邻范围的现有视觉事件。

缺少这些信息时说明缺失项，不从音频或文件名猜测。

## 2. 设计计划必须包含

- [ ] 本段一句话视觉说明。
- [ ] 镜头基线与结束状态。
- [ ] 轨道布局、入场和退场策略。
- [ ] 装饰的 bg/mid/fg 分层与 tag。
- [ ] palette：主色、辅色、强调色、中性色。
- [ ] 每个临时滤镜、Bloom、Flash、Shake、屏幕效果的恢复计划。
- [ ] 与下一段的交接状态。

## 3. 事件语义检查

- [ ] duration 使用拍；angleOffset 每 180° 等于一拍。
- [ ] trackColorAnimDuration 按运行时秒周期处理，不当作拍。
- [ ] AddDecoration 被当作加载时声明，不当作定时生成。
- [ ] Camera/CameraAspect 装饰坐标与 tile 坐标没有混用。
- [ ] MoveDecorations.rotationOffset 按绝对目标角处理。
- [ ] 多 decoration tag 按 OR 并集理解。
- [ ] PositionTrack 用于静态传播布局，MoveTrack 用于运行时动画。
- [ ] AnimateTrack 的 beatsAhead/Behind 按拍理解。
- [ ] ScreenTile 用 `(1,1)` 复位，ScreenScroll 用 `(0,0)` 关闭。
- [ ] Shake 的 strength 是幅度，intensity 主要是频率。
- [ ] 各滤镜 intensity 独立解释。

## 4. 修改前检查

- [ ] revision 仍与计划时一致。
- [ ] 更新/删除引用的 collection、index、type、fingerprint 仍准确。
- [ ] 整批事件已经全量验证。
- [ ] 没有 locked event、preview mode 或 path lock。
- [ ] 同属性 Tween 没有无意重叠。
- [ ] 事件顺序与遮切事务顺序明确。

## 5. 视觉质量检查

- [ ] 当前与下一判定轨道始终可读。
- [ ] 每段只有一个主要视觉动作。
- [ ] 普通强调最多一个主效果加一个低强度辅助。
- [ ] 强效果前后有强度对比。
- [ ] 镜头、轨道和装饰运动方向一致或有明确对位关系。
- [ ] 三层空间有清楚的 depth/parallax 差异。
- [ ] Bloom、Flash、轨道和装饰来自同一 palette。
- [ ] 前景不会长时间遮挡主体。
- [ ] 没有无理由的随机滤镜、随机颜色或随机大旋转。
- [ ] 没有从超密集谱复制极端参数和事件数量。

## 6. 状态泄漏检查

- [ ] 镜头 relativeTo、position、rotation、zoom 有确定结束状态。
- [ ] Flash 最终透明。
- [ ] Bloom 先降强度，再按需禁用。
- [ ] 标准/高级滤镜回到 baseline 或关闭。
- [ ] HallOfMirrors 明确关闭。
- [ ] Shake 已结束。
- [ ] 临时装饰已隐藏或退场。
- [ ] 轨道 position、rotation、scale、opacity 正确交接。
- [ ] ScreenTile/ScreenScroll 已复位。

## 7. 分批预览

建议批次：

1. 镜头和轨道。
2. 背景与中景装饰。
3. 前景、文字与遮罩。
4. 色彩、Bloom 和滤镜。
5. Flash、Shake 和转场强调。

每批之后在编辑器中预览完整进入、保持、退出和下一段开头。不要一次写完整首谱再检查。

## 8. 保存前最终检查

- [ ] 全部预览通过。
- [ ] current revision 与准备保存的内存状态一致。
- [ ] 文件路径存在且为 `.adofai`。
- [ ] 没有未完成的段落状态或临时事件。
- [ ] 已确认不存在意外覆盖谱师事件。
- [ ] 保存后验证写入 revision。

## 9. 明确禁止

- 自行分析音频并猜节奏点。
- 把事件数量当作质量。
- 给普通重音堆满镜头、Flash、Bloom、滤镜、Shake 和轨道移动。
- 把所有滤镜 intensity 统一为 0–100 强度尺度。
- 默认使用 SetFilterAdvanced。
- 依赖下一段“可能会覆盖”当前临时状态。
- 用 PositionTrack 制作连续动画。
- 把 null/禁用通道擅自补成 0。
- 直接改原始 `.adofai` 文本绕开 revision、typed DTO、runtime metadata 和编辑器历史。


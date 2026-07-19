# 闪光、Bloom、滤镜与屏幕效果

## 1. 这些效果的共同职责

光效与后期用于：

- 强调少量关键点。
- 统一段落色彩和材质。
- 遮挡硬切。
- 暂时改变画面感受。

它们不应承担基础构图。没有清楚的镜头、轨道和装饰层时，继续叠滤镜不会让画面更完整。

## 2. Flash

96 个主谱中 95.8% 使用 Flash。前景 Flash 约占 69%，背景约占 31%。最常见的是从一定 opacity 淡到 0：

- duration 中位数 2 拍，75% 不超过 4 拍。
- startOpacity 中位数 20，75% 不超过 50。
- 约 64% 的 Flash 以 endOpacity=0 结束。

### 前景击打闪

- 颜色：本段高光色，缺省才用白色。
- startOpacity：20–50。
- endOpacity：0。
- duration：1–4 拍。
- ease：OutSine/OutCubic/OutQuad。

100% 白闪只留给极少数最高强度点，并保持非常短。

### 背景色洗

- startOpacity：10–25。
- endOpacity：0。
- duration：2–4 拍。
- 使用本段主色或辅色。

背景色洗不遮挡主体，适合给普通强调增加色彩响应。

### 黑场与遮切

- 前景黑色瞬时建立。
- 在遮挡期间切换镜头、背景、轨道色和装饰组。
- 0.5–4 拍淡出。
- endOpacity 必须明确为 0。

同一 plane 的新 Flash 会结束旧 Tween，连续 Flash 不能当作自然叠加层。

## 3. Bloom

Bloom 用于统一亮度与颜色，不是每拍爆亮：

- 样本 intensity 中位数约 200，75% 约 500。
- 常见 threshold 为 0、1、10、15、50。
- Bloom color 经常跟随段落 palette，而非永远白色。

可靠用法：

- 段落进入时用 2–4 拍过渡到新颜色和强度。
- 强调点可以瞬时抬高，再用 1–4 拍回到本段基线。
- 退场时先保持 enabled=true，把 intensity 补间到低值；之后再单独禁用。

`enabled=false` 会立即关组件，不会按 duration 淡出。

## 4. 标准滤镜

使用频率最高的滤镜是 Fisheye、Aberration、Grayscale、Waves、Grain、BlurFocus、GaussianBlur 和 Contrast。但 intensity 没有跨滤镜统一含义：Fisheye 的常见值和 MotionBlur、Sharpen、Waves 完全不可比较。

### 滤镜角色

- Fisheye：空间冲击、推拉感。
- Aberration：色散冲击、数字感。
- Grayscale/Sepia：段落色彩收束、回忆感。
- Grain/VHS/Static/Compression：材质与媒介感。
- Blur/GaussianBlur/BlurFocus：失焦、转场、蓄力。
- Waves/Glitch：短暂破坏和故障感。
- Contrast/Sharpen：画面质感调整，容易过量。

### 两步冲击模板

1. duration=0 设置强值。
2. duration=1–2，用 OutQuad/OutCubic 回到该滤镜的已知 baseline。

不能硬编码“50 就是中等”。必须读取该滤镜的运行时 metadata、合法范围、中性值和当前值。

### 关闭规则

许多滤镜在 `enabled=false` 时立即关闭。需要平滑离开时：

1. 保持 enabled=true，补间到中性或低强度。
2. 补间结束后再禁用。

`disableOthers=true` 会立刻关闭其他滤镜，应只在明确接管整套后期状态时使用。

## 5. SetFilterAdvanced

96 个主谱中只有 8 个使用，且大部分事件集中在极少数实验谱。它适合明确的 glitch、EarthQuake、TV Distorted、WideScreen 等主题，不是默认能力。

使用前必须具备：

- 确切滤镜类型。
- 类型化字段 schema。
- 每个字段的单位、中性值和合法范围。
- 显式关闭事件。

不得向 AI 暴露任意字符串属性字典并让它猜字段。

## 6. ShakeScreen

- `strength` 决定主要位移幅度。
- `intensity` 主要决定震动次数，不是幅度。
- 常用 duration 为 1–4 拍。
- `fadeOut=true` 更适合普通冲击。
- Out* ease 使震动从强到弱，In* 使震动逐渐增强，InOut* 形成完整包络。

普通击打只需短、轻、可恢复的 Shake。长时间高强度震动会破坏读轨，也会与运镜争夺控制权。

## 7. HallOfMirrors

只有约三分之一主谱使用。它是段落 gimmick，不是基础效果：

- 成对开启和关闭。
- 明确限定 floor 范围。
- 开启期间减少其他失真效果。
- 不能把关闭责任留给后续不确定事件。

## 8. ScreenTile 与 ScreenScroll

### ScreenTile

- `(1,1)` 是中性/关闭状态。
- 参数不是百分比。
- 可按 duration 补间。
- 适合短暂分屏、平铺和结构性失真。

结束时必须回到 `(1,1)`，不能用 `(0,0)` 代表关闭。

### ScreenScroll

- 是持续 UV 速度，没有 duration 和 ease。
- `(0,0)` 关闭。
- `scroll=(100,0)` 约表示每秒一个归一化纹理周期，而非移动到 x=100。

每次启用都要有明确停止点。

## 9. 后期自检

- 这个效果是在强调构图，还是在掩盖缺少构图？
- intensity 是否按具体滤镜解释？
- 临时效果是否有 baseline 与关闭计划？
- Bloom 是否被直接 enabled=false 截断？
- Flash 是否回到透明？
- Shake 的 strength 和 intensity 是否被混淆？
- HallOfMirrors、ScreenTile、ScreenScroll 是否显式复位？
- 强后期是否覆盖了需要读轨的长段？


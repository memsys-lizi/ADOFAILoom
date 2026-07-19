# Workshop 视效研究

## 1. 研究范围

数据源：

```text
D:\Steam\steamapps\workshop\content\977950
```

扫描结果：

- 96 个 Workshop item。
- 203 个 `.adofai` 文件。
- 96 个 `main.adofai`，107 个 sub/其他变体。
- 总大小 49,979,994 字节，约 47.66 MiB。
- 190,432 条 actions。
- 16,710 条 decorations。

最终风格统计按 Workshop item 等权：每个 item 只取 `main.adofai`，避免一个多章节包因包含大量 sub 谱而重复加权。sub 谱用于格式与技法核对。

## 2. 数据格式事实

203 个文件中只有 86 个是严格 JSON：

- 64 个包含尾逗号。
- 36 个在 JSON 字符串中包含原始控制字符。
- 14 个包含游离或重复逗号。
- 3 个存在缺逗号或分隔符问题。

研究时使用只读、内存中的游戏兼容解析覆盖全部 203 个文件。非法 JSON 写法只是历史格式事实，不是生成规范；任何新事件必须通过编辑器和项目现有类型化接口写入。

## 3. 主谱事件覆盖率

| 事件 | 事件数 | 使用主谱 | 覆盖率 |
|---|---:|---:|---:|
| MoveCamera | 25,436 | 96 | 100.0% |
| MoveTrack | 35,593 | 94 | 97.9% |
| Flash | 5,839 | 92 | 95.8% |
| SetFilter | 13,487 | 86 | 89.6% |
| AddDecoration | 17,669 | 76 | 79.2% |
| MoveDecorations | 58,991 | 74 | 77.1% |
| AnimateTrack | 1,323 | 70 | 72.9% |
| Bloom | 2,318 | 69 | 71.9% |
| RecolorTrack | 3,357 | 68 | 70.8% |
| CustomBackground | 1,084 | 66 | 68.8% |
| PositionTrack | 2,695 | 64 | 66.7% |
| HallOfMirrors | 226 | 32 | 33.3% |
| SetFilterAdvanced | 623 | 8 | 8.3% |

结论：稳定骨架是相机、轨道、闪光与装饰；高级滤镜只属于少数作品的特殊语言。

## 4. 事件组合与节制

主谱视觉活跃 floor 共 43,226 个：

- 65.5% 只有一种视觉事件类型。
- 19.1% 有两种。
- 7.5% 有三种。
- 只有约 7.9% 超过三种。

最常见的同 floor 组合包括：

- MoveCamera + MoveTrack。
- MoveCamera + MoveDecorations。
- Flash + MoveCamera。
- MoveDecorations + MoveTrack。
- MoveCamera + SetFilter。

优质谱可以有极复杂的高潮，但普通视觉节点仍以一到两个通道为主。

## 5. 长尾与偏差

- SetFilterAdvanced 的 62.9% 来自一个谱，前三个谱贡献 95.3%。
- MoveDecorations 的 39% 来自 `2924697157`，前三个谱贡献 53.9%。
- 少数生成型谱包含几十万级位置偏移、极端滤镜值和数万装饰移动事件。

因此不能用事件总数或最大参数判断质量，也不能把全量均值直接作为推荐值。指南优先使用 item 覆盖率、中位数、四分位和跨作品重复出现的结构。

## 6. 代表性主谱

### `2301231290/main.adofai` — R

运镜克制，zoom 主要围绕 130，1–2 拍完成；蓝色 Bloom、Aberration 与 MotionBlur 形成统一语言。适合学习常规击打运镜和单色调后期体系。

### `2731817282/main.adofai` — MAKE IT FUNKY NOW

大量 RecolorTrack，几乎不依赖普通滤镜、Bloom 或大规模装饰。用 Single/Blink/Stripes/Glow 和轨道样式形成视觉节奏。证明单一强语法可以比堆效果更鲜明。

### `2833658365/main.adofai` — Jazz Candy (VIP)

稳定 Player 镜头基线，使用指数 ease 制造弹性冲击；背景 Flash 与文字承担段落主题。适合学习稳定基线、冲击 ease 和文字段落卡。

### `2899389533/main.adofai` — Witch of the Hollows

相机、滤镜、Bloom、轨道与分层装饰同步切换，并集中清理旧段落。适合学习复杂段落状态机与完整清场，不适合复制事件密度和极端参数。

### `2924697157/main.adofai` — PLUM MEGAMIX 2

使用段落化 tag 隔离大量素材。最值得学习的是命名空间和团队段落资源隔离；数万 MoveDecorations 和超长 tag 列表不是通用模板。

### `2967497446/main.adofai` — 1,2,3,4!

大量装饰和多级 parallax，通过图形帧与纵深驱动画面。适合学习素材动画和装饰层次，而不是滤镜堆叠。

### `2975797214/main.adofai` — LORELEI

大量短镜头使用“瞬时建立 → OutCubic 稳定”，配 letterbox、vignette、前景 Flash 和显式关闭旧滤镜。适合学习电影式转场事务。

### `3012389896/main.adofai` — Fragor Magnus

背景图、背景色、Bloom、轨道色和雾层成套变化；轨道以小范围位移和旋转退场。适合学习统一调色与整体换景。

### `3296283989/main.adofai` — Avantgarde

丰富的 blend mode、发光粒子、前中后景和电影画幅。适合学习主题化材质与宽屏构图，但高级滤镜仍是主题服务项。

### `3371824643/main.adofai` — REBIRTH

几何装饰与 Fisheye 形成核心 motif，其他色彩变化反而克制。适合学习让一个可识别 motif 贯穿作品。

### `3386617648/main.adofai` — deleted

高级滤镜实验样本，适合建立独立的高级滤镜参考库，不适合作为默认审美基线。

### `3540508961/main.adofai` — Light Years Away

围绕行星的装饰、字形、几何图案和多级 parallax，配合段落清场。适合学习行星相对装饰和排版式视效。

## 7. 由样本支持的关键数字

### MoveCamera

- duration 中位数约 1 拍，75% 不超过 2 拍。
- zoom 中位数约 150，主要四分位约 120–180。
- 大部分显式 rotation 为 0，小角度主要集中在 5–15°。
- 7,061 个 floor 上有至少两个 MoveCamera，最常见结构是 duration `0 → 1/2`。

### Flash

- 前景 4,054，背景 1,836。
- duration 中位数 2 拍。
- startOpacity 中位数 20。
- endOpacity 最常为 0。

### Track

- AnimateTrack beatsAhead 中位数 3，常用 3–6。
- beatsBehind 常用 0–4。
- MoveTrack duration 中位数 4，常用 1–8。
- PositionTrack 90% 的轴偏移大致不超过 3 tile。

### Decoration

- 92% 以上 AddDecoration 带 tag。
- 初始 opacity 0 与后续 tag 动画是最常见生命周期。
- 常见 parallax 档位为 0、25、50、80、99/100 附近。

## 8. 研究结论的使用方式

这些数字用于给 AI 建立尺度感和优先级，不用于自动生成固定模板。每次实际制作仍需：

1. 由谱师给出 floor 与视觉意图。
2. 读取当前关卡状态和已有事件。
3. 选择一个段落级视觉语法。
4. 用知识库中的尺度作为初始方案。
5. 在游戏内预览并迭代。


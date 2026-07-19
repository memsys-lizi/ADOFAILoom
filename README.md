# ADOFAILoom MCP Mod

ADOFAILoom 在《A Dance of Fire and Ice》游戏进程内提供通用 MCP 工具。Mod 启用后直接开放标准 Streamable HTTP 端点，不需要 EXE、命名管道或额外 .NET 运行时，也不包含 Codex、Cursor、Claude 的专用逻辑。

```text
Codex / Cursor / Claude
          │ Streamable HTTP
          ▼
http://127.0.0.1:39473/mcp
          │
ADOFAILoom.dll（Unity Mod）
          │ Unity 主线程
          ▼
       游戏状态
```

游戏没有运行、Mod 没有启用或端口被占用时，MCP 端点不可连接。客户端不会启动任何 ADOFAILoom 进程。

## 安装

发行目录只有两个文件：

```text
out/ADOFAILoom/
├─ ADOFAILoom.dll
└─ Info.json
```

将整个 `ADOFAILoom` 文件夹复制到游戏的 UMM Mods 目录：

```text
A Dance of Fire and Ice/Mods/ADOFAILoom/
```

启动游戏并启用 ADOFAILoom 后，服务监听：

```text
http://127.0.0.1:39473/mcp
```

建议先启动游戏，再让 MCP 客户端连接或刷新工具列表。

## 当前工具

### `get_game_state`

获取当前游戏状态，无输入参数：

```json
{
  "schemaVersion": 2,
  "scene": "scnEditor",
  "mode": "editor",
  "paused": true
}
```

- `mode`：`menu | level_select | editor | gameplay | unknown`
- 未识别场景：保留真实 `scene`，`mode=unknown`
- 当前场景没有 `scrController`：`paused=null`
- 游戏或 Mod 不可用：HTTP 端点不可连接，不返回伪造状态

工具调用同时返回 JSON 文本内容和 `structuredContent`。工具被标记为只读、非破坏、幂等且不访问开放世界。

### `switch_scene`

切换到准确的 Unity 场景名：

```json
{
  "sceneName": "scnEditor"
}
```

常见场景名包括：

- `scnEditor`：关卡编辑器
- `scnCLS`：自定义关卡选择
- `scnLevelSelect`：官方选关

工具会验证场景是否真实存在于当前游戏安装中，不提供别名、拼写修正或替代场景。返回：

```json
{
  "sceneName": "scnEditor",
  "status": "started"
}
```

如果编辑器存在未保存内容，游戏会显示自己的确认窗口，此时返回 `status=awaiting_confirmation`。工具被标记为修改状态、破坏性、非幂等且不访问开放世界。

### `open_level`

在当前关卡编辑器中打开现有 `.adofai` 文件：

```json
{
  "levelPath": "D:\\Levels\\Example\\main.adofai"
}
```

- 必须传入绝对文件路径。
- 文件必须存在且扩展名必须为 `.adofai`。
- 当前场景必须是 `scnEditor`；工具不会自动切换场景，也不会把参数解释成官方关卡 ID。
- 有未保存内容时返回 `status=awaiting_confirmation`，否则返回 `status=started`。

该工具会读取用户指定的本地文件并替换当前编辑器内容，因此标记为修改状态、破坏性、非幂等和开放世界。

### 编辑器状态与 revision

`get_editor_state` 返回当前编辑器文档状态：

```json
{
  "levelPath": "D:\\Levels\\Example\\main.adofai",
  "floorCount": 128,
  "selectedFloors": [64],
  "selectedDecorations": [],
  "currentFloor": 64,
  "previewing": false,
  "canUndo": true,
  "canRedo": false,
  "revision": "64位小写SHA-256"
}
```

除读取工具外，所有编辑器工具都要求传入当前 `expectedRevision`。Revision 来自规范化后的完整关卡数据；谱师在编辑器中手工修改关卡后，旧 revision 会立即失效，客户端必须重新读取状态。

`list_visual_events` 分页读取游戏元数据归类为轨道、装饰或视觉效果的事件：

```json
{
  "floorStart": 60,
  "floorEnd": 80,
  "eventTypes": ["MoveCamera", "SetFilter"],
  "offset": 0,
  "limit": 100
}
```

返回项包含其真实集合、索引、事件类型和内容指纹：

```json
{
  "collection": "LevelEvents",
  "index": 12,
  "eventType": "MoveCamera",
  "fingerprint": "64位小写SHA-256"
}
```

事件引用不会自动追踪、重定位或寻找相似事件。集合内容变化后，过期引用会明确失败。

### 镜头视效

镜头工具为：

- `add_camera_move_events`
- `update_camera_move_events`
- `remove_camera_move_events`

新增接口接受 1–256 个事件并保持数组顺序：

```json
{
  "expectedRevision": "当前revision",
  "events": [
    {
      "floor": 64,
      "angleOffset": 0,
      "duration": 2,
      "ease": "OutCubic",
      "position": { "x": 1.5, "y": 0 },
      "rotation": 8,
      "zoom": 120,
      "relativeTo": "Player"
    }
  ]
}
```

- `floor` 是从零开始的楼层；`angleOffset` 单位为度；`duration` 单位为拍。
- 位置使用格，旋转使用度，缩放使用百分比。
- `position`、`rotation`、`zoom`、`relativeTo` 至少提供一项；没有提供的属性保持禁用。
- 更新项使用 `list_visual_events` 返回的 `reference`。只有明确提供的字段会改变；`disabledProperties` 可包含 `Position | Rotation | Zoom | RelativeTo`。
- 同一更新不能同时为某个属性提供值并将其禁用。

### 普通滤镜

普通滤镜工具为：

- `add_filter_events`
- `update_filter_events`
- `remove_filter_events`

```json
{
  "expectedRevision": "当前revision",
  "events": [
    {
      "floor": 64,
      "angleOffset": 0,
      "filter": "Grayscale",
      "enabled": true,
      "intensity": 75,
      "disableOthers": false,
      "duration": 1,
      "ease": "InOutSine"
    }
  ]
}
```

滤镜名、缓动名、强度和持续时间严格按当前游戏事件元数据验证，不纠正大小写、不 trim、不 clamp。当前版本只支持标准 `SetFilter`，不接受 `SetFilterAdvanced` 的动态属性。

### 撤销、重做与保存

- `undo_editor(expectedRevision)` 调用编辑器现有撤销栈。
- `redo_editor(expectedRevision)` 调用编辑器现有重做栈。
- `save_level(expectedRevision)` 将当前内存状态写入已经打开的绝对 `.adofai` 路径，并重新读取文件校验 revision。

一个成功的批量视效调用对应一个编辑器撤销步骤。新增、更新和删除不会自动保存；当前关卡没有可用路径时，`save_level` 明确失败，不弹出另存为窗口。

所有视觉写入只接受谱师或模型明确给出的游戏原生时点，不分析音乐、推测 BPM、生成落砖或调整节奏。

## 客户端配置

所有客户端都连接同一个 URL，并通过 MCP `initialize` 与 `tools/list` 自动发现服务能力和工具定义。

### Codex

```toml
[mcp_servers.adofai]
url = "http://127.0.0.1:39473/mcp"
```

### Cursor

```json
{
  "mcpServers": {
    "adofai": {
      "url": "http://127.0.0.1:39473/mcp"
    }
  }
}
```

### Claude Code

```powershell
claude mcp add --transport http adofai http://127.0.0.1:39473/mcp
```

删除客户端中旧的 `command = ...ADOFAILoom.McpServer.exe` 或 stdio 配置。修改配置后按客户端要求重新连接或刷新 MCP 服务。

### 协议兼容性

服务端不根据客户端名称或 `User-Agent` 分支处理，而是统一实现 MCP 请求契约：

- 接受并校验请求参数中的标准 `_meta` 对象，包括 Cursor 使用的 `progressToken`。
- `_meta` 中未被服务端使用的扩展键会按 MCP 约定保留为不透明元数据，不会被误判成工具参数。
- 无参数工具接受省略 `arguments`、`arguments: {}`，以及客户端将可选参数对象序列化成 `arguments: null` 的形式；该归一化只发生在 `tools/call` 外层，具体工具参数仍严格按照生成的 JSON Schema 校验。
- 未知业务参数、错误的 `_meta` 类型、错误的工具参数类型和未协商的协议功能仍会明确失败。

当前协议层面向 Codex、Cursor 和 Claude Code 的 Streamable HTTP 客户端；三者使用同一个 URL 和同一组自动发现、调用流程。

## 新增工具

在 `src/Mod/Mcp/Tools/` 新建一个工具类，使用构造函数声明依赖，并给业务方法添加 `[McpTool]`：

```csharp
internal sealed class ExampleTool
{
    [McpTool(
        "example_tool",
        Description = "Describe what the tool returns.",
        ReadOnly = true,
        Destructive = false,
        Idempotent = true,
        OpenWorld = false)]
    public ExampleResult Execute(string value, CancellationToken cancellationToken)
    {
        return new ExampleResult(value);
    }
}
```

启动时会由 `Mcp/Tooling/` 中的框架自动反射发现工具，并生成工具清单、参数 JSON Schema、annotations 和调用适配器。普通方法参数会成为 camelCase MCP 参数；`CancellationToken` 由框架注入。`Mcp/Tools/` 只存放具体工具，工具方法只返回业务 DTO，不手写 JSON-RPC 或 MCP 响应。

如果工具类需要新的构造函数依赖，必须在 `McpServerFactory` 中显式注册。重复工具名、缺少描述、未知依赖、非法签名或不支持的参数类型都会阻止 Mod 启用，不会被忽略。

## 源码结构

```text
ADOFAILoom/
├─ ADOFAILoom.sln
├─ AGENTS.md
├─ README.md
├─ build/
│  └─ ADOFAILoom.Mod.targets
└─ src/Mod/
   ├─ ADOFAILoom.Mod.csproj
   ├─ Info.json
   ├─ Main.cs
   ├─ Actions/
   ├─ Mcp/
   │  ├─ Protocol/
   │  ├─ Tooling/
   │  ├─ Tools/
   │  └─ Transport/
   ├─ State/
   ├─ Threading/
   └─ Properties/
```

仓库只有一个解决方案、一个项目和一个运行时 DLL。

## 构建与部署

本机游戏路径放在被 Git 忽略的 `ADOFAILoom.local.props`，也可以使用 `ADOFAI_GAME_EXE` 环境变量或 `GameExePath` 参数。

在仓库根目录只构建安装包：

```powershell
dotnet build -c Release -p:DeployMod=false
```

构建并部署到游戏目录：

```powershell
dotnet build -c Release -p:DeployMod=true
```

部署会重建 `Mods/ADOFAILoom`，保证安装目录只保留当前发行文件。默认不会启动游戏；只有同时显式传入 `-p:AutoLaunchGame=true` 才会启动。

## 手工验证

启动游戏并启用 Mod 后，HTTP POST 必须包含：

```text
Content-Type: application/json; charset=utf-8
Accept: application/json, text/event-stream
MCP-Protocol-Version: 2025-11-25  # initialize 之后的请求
```

依次验证 `initialize`、`notifications/initialized`、`tools/list` 和 `tools/call`。服务显式支持 `2025-03-26`、`2025-06-18`、`2025-11-25`。

还应手工确认：

- GET 和 DELETE 返回 405；非 loopback Origin 返回 403。
- 非 JSON、缺失 Accept、缺失协议版本、超过 64 KiB 和非法参数明确失败。
- 主菜单、选关、自定义选关、编辑器、游玩和暂停状态正确。
- `switch_scene` 能切换 `scnEditor`、`scnCLS` 等有效场景，并拒绝不存在的场景。
- `open_level` 能打开有效 `.adofai`，并拒绝相对路径、错误扩展名、不存在的文件和非编辑器场景。
- 编辑器有未保存内容时，两项操作返回 `awaiting_confirmation` 并交由游戏确认窗口决定是否继续。
- `get_editor_state` 和 `list_visual_events` 在非编辑器场景、编辑器加载中明确失败，并返回一致的当前 revision。
- 镜头和滤镜批量新增保持请求顺序且只产生一个撤销步骤；更新和删除拒绝过期、错误类型、错误集合及锁定事件引用。
- 所有视觉参数拒绝错误大小写、未知嵌套字段、越界数值、空批次和不支持的首楼层位置，不执行 trim、clamp 或替代查找。
- `undo_editor`、`redo_editor` 正确更新 revision；`save_level` 只在磁盘 JSON 与当前 revision 一致时报告成功。
- 禁用或卸载 Mod 后端点立即停止。

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
- 禁用或卸载 Mod 后端点立即停止。

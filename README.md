# ADOFAILoom MCP Mod

ADOFAILoom 在《A Dance of Fire and Ice》游戏进程内直接提供标准 Streamable HTTP MCP 服务。它不需要独立 EXE、伴随进程或命名管道。

```text
Codex / Cursor / Claude ──HTTP──> http://127.0.0.1:39473/mcp ──> 游戏内 Mod
```

游戏关闭或 Mod 被禁用时，MCP 地址自然离线；游戏启动并启用 Mod 后，MCP 服务随之启动。

## 工具

首版只有只读工具 `get_game_state`：

```json
{
  "schemaVersion": 1,
  "connected": true,
  "scene": "scnEditor",
  "mode": "editor",
  "paused": true
}
```

`mode` 为 `menu`、`level_select`、`editor`、`gameplay` 或 `unknown`。没有游戏控制器的场景中，`paused=null`。

## 源码结构

仓库只有一个解决方案和一个 Mod 项目，所有 C# 源码都在 `src/Mod`：

```text
src/Mod/
├─ Main.cs       # UMM 生命周期
├─ Mcp/          # Streamable HTTP、JSON-RPC、工具注册
│  └─ Tools/     # 每个 MCP 工具的独立实现
├─ State/        # 游戏状态 DTO 与主线程状态采集
└─ Threading/    # Unity 主线程调度
```

新增工具时，在 `Mcp/Tools` 中实现 `IMcpTool`，并在 `McpServerFactory` 的注册表中加入一行注册。需要读取 Unity 对象的逻辑放在 `State`，通过通用的 `MainThreadDispatcher.InvokeAsync` 在游戏主线程执行。

## 构建与部署

本机游戏路径放在被 Git 忽略的 `ADOFAILoom.local.props`，也可通过 `ADOFAI_GAME_EXE` 或 `GameExePath` 指定。

构建但不部署：

```powershell
dotnet build .\ADOFAILoom.csproj -c Release
```

部署到游戏 `Mods\ADOFAILoom`：

```powershell
dotnet build .\ADOFAILoom.csproj -c Release -p:DeployMod=true
```

部署后重新启动游戏，并在 Unity Mod Manager 中启用 `ADOFAILoom MCP Server`。UMM 日志应出现：

```text
MCP server listening at http://127.0.0.1:39473/mcp
```

## 客户端配置

只有游戏运行且 Mod 启用时，客户端才能连接。

### Codex

```powershell
codex mcp remove adofai
codex mcp add adofai --url http://127.0.0.1:39473/mcp
```

等价的 `~/.codex/config.toml`：

```toml
[mcp_servers.adofai]
url = "http://127.0.0.1:39473/mcp"
```

### Cursor

`.cursor/mcp.json` 或用户级 MCP 配置：

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

配置后重启对应客户端或扩展，让它重新初始化 MCP 服务。

## HTTP 行为与安全

- 只监听 IPv4 回环地址 `127.0.0.1`，不会暴露到局域网。
- 单一 MCP 端点为 `/mcp`，`POST` 返回 JSON；本服务不需要服务器推送，因此 `GET` 返回 405。
- 校验非空 `Origin`，只接受 loopback 来源，降低 DNS rebinding 风险。
- 支持 MCP 协议版本 `2025-03-26`、`2025-06-18` 和 `2025-11-25`。
- 请求体上限 64 KiB，游戏主线程状态读取超时为 5 秒。

## 手工验证

游戏启动后可先检查端点是否在线：

```powershell
Invoke-RestMethod -Method Post `
  -Uri http://127.0.0.1:39473/mcp `
  -ContentType 'application/json' `
  -Headers @{ Accept='application/json, text/event-stream' } `
  -Body '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"manual","version":"1.0"}}}'
```

随后在 MCP 客户端调用 `get_game_state`，分别检查菜单、选关、编辑器、游玩和暂停状态。

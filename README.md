# ADOFAILoom MCP Mod

ADOFAILoom 为《A Dance of Fire and Ice》提供通用 MCP 工具。发行包同时包含游戏 Mod 和标准 stdio MCP Server，不依赖 Codex、Cursor 或 Claude 的专用协议。

```text
Codex / Cursor / Claude
          │ stdio
          ▼
ADOFAILoom.McpServer.exe
          │ Windows 命名管道
          ▼
ADOFAILoom.dll（游戏内 Mod）
          │ Unity 主线程
          ▼
      游戏状态
```

## 安装包

构建产生可直接安装的完整 Mod 目录：

```text
out/ADOFAILoom/
├─ ADOFAILoom.dll
├─ ADOFAILoom.McpServer.exe
└─ Info.json
```

`ADOFAILoom.McpServer.exe` 是 Windows x64 自包含单文件程序，用户不需要另外安装 .NET。发布时将整个 `ADOFAILoom` 文件夹放进游戏的 `Mods` 目录：

```text
A Dance of Fire and Ice/Mods/ADOFAILoom/
```

MCP 客户端配置必须引用安装后的 `Mods/ADOFAILoom/ADOFAILoom.McpServer.exe`，不要引用开发仓库中的 `bin`、`obj` 或临时发布目录。

## 工具

当前只有只读工具 `get_game_state`：

```json
{
  "schemaVersion": 1,
  "connected": true,
  "scene": "scnLevelSelect",
  "mode": "level_select",
  "paused": false
}
```

- `mode`：`menu | level_select | editor | gameplay | unknown | null`
- 游戏未运行或 Mod 未启用：`connected=false`，其他状态字段为 `null`
- 当前场景没有 `scrController`：`paused=null`

## 源码结构

仓库只有一个解决方案和两个必要项目，没有测试项目和额外协议项目：

```text
ADOFAILoom/
├─ ADOFAILoom.sln                   # 唯一解决方案
├─ AGENTS.md
├─ README.md
├─ build/
│  └─ ADOFAILoom.Mod.targets        # 打包与部署逻辑
└─ src/
   ├─ Mod/
   │  ├─ ADOFAILoom.Mod.csproj      # 游戏内 Mod 项目
   │  ├─ Info.json
   │  ├─ Properties/
   │  ├─ Main.cs
   │  ├─ Bridge/                    # 游戏内命名管道服务
   │  ├─ State/                     # Unity 状态采集
   │  └─ Threading/                 # Unity 主线程调度
   ├─ McpServer/
   │  ├─ ADOFAILoom.McpServer.csproj
   │  ├─ Program.cs
   │  ├─ Bridge/                    # 命名管道客户端
   │  └─ Tools/                     # 官方 SDK 工具定义
   └─ Shared/
      └─ Protocol/                  # 共享源码，不是第三个项目
```

MCP Server 使用官方 `ModelContextProtocol 1.4.1` C# SDK。新增工具时，在 `src/McpServer/Tools` 增加带 `[McpServerTool]` 的方法；需要读取游戏对象时，在 Mod 的 `State` 中采集，并通过共享管道协议返回。

## 构建与部署

本机游戏路径放在被 Git 忽略的 `ADOFAILoom.local.props`，也可以通过 `ADOFAI_GAME_EXE` 或 `GameExePath` 指定。

只构建安装包，不写入游戏目录：

```powershell
dotnet build .\ADOFAILoom.sln -c Release -p:DeployMod=false
```

构建并复制到游戏的 `Mods/ADOFAILoom`：

```powershell
dotnet build .\ADOFAILoom.sln -c Release -p:DeployMod=true
```

默认不会启动游戏；只有额外设置 `-p:AutoLaunchGame=true` 才会启动。

## MCP 客户端配置

下面以游戏安装在 `D:\Games\A Dance of Fire and Ice` 为例。请替换成用户自己的实际安装路径。

### 通用 JSON / Cursor

```json
{
  "mcpServers": {
    "adofai": {
      "command": "D:\\Games\\A Dance of Fire and Ice\\Mods\\ADOFAILoom\\ADOFAILoom.McpServer.exe",
      "args": []
    }
  }
}
```

### Codex

```toml
[mcp_servers.adofai]
command = 'D:\Games\A Dance of Fire and Ice\Mods\ADOFAILoom\ADOFAILoom.McpServer.exe'
```

也可以使用命令行：

```powershell
codex mcp remove adofai
codex mcp add adofai -- "D:\Games\A Dance of Fire and Ice\Mods\ADOFAILoom\ADOFAILoom.McpServer.exe"
```

### Claude Code

```powershell
claude mcp add --transport stdio adofai -- "D:\Games\A Dance of Fire and Ice\Mods\ADOFAILoom\ADOFAILoom.McpServer.exe"
```

客户端会按需启动这个 EXE。游戏未运行时 MCP Server 仍能启动，`get_game_state` 会返回 `connected=false`；游戏启动并启用 Mod 后，后续调用会直接读取实时状态。

## 手工验证

1. 确认 `out/ADOFAILoom` 只有三个发行文件。
2. 配置客户端使用安装目录中的 EXE，并重启客户端使 MCP 配置生效。
3. 游戏未启动时调用 `get_game_state`，确认 `connected=false`。
4. 启动游戏并启用 Mod，检查菜单、选关、编辑器、游玩及暂停状态。
5. 禁用 Mod 后再次调用，确认恢复为 `connected=false`。

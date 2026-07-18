# ADOFAILoom repository rules

## Purpose and architecture

This repository implements a vendor-neutral MCP server inside the A Dance of Fire and Ice Unity process. The Unity Mod Manager mod exposes a Streamable HTTP endpoint at `http://127.0.0.1:39473/mcp`.

There is one solution, one `netstandard2.1` project, and no standalone MCP executable. Do not reintroduce a sidecar process, stdio server, named-pipe bridge, ASP.NET host, or additional project unless the user explicitly reverses this architecture decision.

## Non-negotiable rules

- Keep every C# source file under `src/Mod/`.
- Do not create test projects, test directories, test files, fixtures, mocks, snapshots, or other automated test code. Verify with builds, direct HTTP protocol smoke checks, UMM logs, and manual in-game checks.
- Keep the MCP contract client-neutral. Codex-, Cursor-, and Claude-specific content belongs in documentation only.
- Never commit a machine-specific game path. Use `ADOFAI_GAME_EXE`, `GameExePath`, or the ignored `ADOFAILoom.local.props`.
- Normal builds must not deploy or launch the game. Deployment requires `DeployMod=true`; launching additionally requires `AutoLaunchGame=true`.

## Source layout

```text
src/Mod/
├─ Main.cs       UMM lifecycle coordinator
├─ Mcp/          Streamable HTTP transport, MCP JSON-RPC handling, tool registry
│  └─ Tools/     one independent implementation per MCP tool
├─ State/        state DTOs, mode mapping, Unity state capture
└─ Threading/    Unity main-thread dispatcher
```

- HTTP/background code must not reference Unity state directly.
- Every MCP tool must implement `IMcpTool` in its own file and be registered in `McpServerFactory`.
- State capture is the only layer that may read `ADOBase`, `scrController`, `scnGame`, or `scnEditor`.
- Pure state DTOs and mapping logic must not depend on networking or UMM.

## Unity lifecycle and threading

- Start the HTTP listener when the Mod is enabled and stop it when disabled or unloaded.
- Never access Unity objects from `HttpListener`, task-pool, or request threads.
- Enqueue game reads through `MainThreadDispatcher` and execute them in UMM `OnUpdate`.
- Cancel pending work and dispose the listener, active responses, tasks, and cancellation sources during shutdown.
- Do not add Harmony patches unless stable game APIs cannot implement a required feature; document the reason if that changes.

## Streamable HTTP rules

- Bind only to `127.0.0.1`, never `0.0.0.0` or a LAN interface.
- Public endpoint: `http://127.0.0.1:39473/mcp`.
- Use one JSON-RPC message per HTTP POST. Return `application/json` for requests and HTTP 202 with no body for accepted notifications.
- This server has no server-to-client messages; GET and DELETE return HTTP 405.
- Validate every non-empty `Origin` and allow loopback origins only.
- Preserve the 64 KiB request limit and 5-second main-thread timeout unless a documented requirement changes them.
- Supported protocol versions are `2025-03-26`, `2025-06-18`, and `2025-11-25`.
- Do not introduce stateful MCP sessions until a feature actually requires server-to-client requests, resumability, or session state.

## MCP contract rules

- Public tool name: `get_game_state`.
- Current state schema version: `1`.
- Preserve field names and meanings. Additive fields may retain the schema version; breaking changes require a new version and an explicit compatibility path.
- Mark read-only tools through MCP annotations with `readOnlyHint=true`, `destructiveHint=false`, `idempotentHint=true`, and `openWorldHint=false`.
- Return both text content and `structuredContent` for successful tool calls.
- Use JSON-RPC errors for malformed requests, unsupported methods, and invalid parameters. Use tool results with `isError=true` for failures during tool execution.

## Code standards

- Use nullable reference types and cancellation-aware asynchronous APIs.
- Keep classes small and responsibilities explicit. `Main` coordinates lifecycle only.
- Use camelCase JSON and keep DTOs serialization-friendly.
- Handle expected disconnect and cancellation paths quietly; log unexpected listener or request failures through UMM.
- Avoid new runtime dependencies. The Mod relies on Unity/Mono BCL APIs and the game-provided `System.Text.Json`.
- Update `README.md` when the endpoint, port, protocol support, tool contract, build command, or client configuration changes.

## Verification without test code

```powershell
dotnet build .\ADOFAILoom.csproj -c Release -p:DeployMod=false
dotnet build .\ADOFAILoom.csproj -c Release -p:DeployMod=true
```

After launching the game and enabling the Mod:

- POST `initialize`, `notifications/initialized`, `tools/list`, and `tools/call` to `/mcp`.
- Confirm GET `/mcp` returns 405 and a non-loopback `Origin` returns 403.
- Confirm menu, level select, editor, gameplay, pause, disable, and shutdown behavior manually.

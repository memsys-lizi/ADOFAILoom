# ADOFAILoom repository rules

## Purpose and architecture

ADOFAILoom is distributed as one installable Unity Mod Manager folder containing both the game Mod and a vendor-neutral stdio MCP Server:

```text
MCP client -> ADOFAILoom.McpServer.exe -> Windows named pipe -> game Mod -> Unity main thread
```

The repository has one solution and exactly two necessary projects:

- `src/Mod/ADOFAILoom.Mod.csproj`: `netstandard2.1` UMM Mod.
- `src/McpServer/ADOFAILoom.McpServer.csproj`: `net8.0` stdio MCP Server using the official C# SDK.

Shared DTOs and named-pipe protocol sources live in `src/Shared` and are compiled directly into both projects. Do not create a third shared project or a shared protocol DLL.

## Non-negotiable rules

- Keep every C# source file under `src/`.
- Keep the repository root limited to the solution and repository-wide documentation or configuration. Project-specific files belong with their project; build targets belong under `build/`.
- Do not create test projects, test directories, test files, fixtures, mocks, snapshots, or other automated test code.
- Do not add Streamable HTTP, ASP.NET, a second sidecar, or another transport unless the user explicitly changes the architecture.
- Keep the MCP contract client-neutral. Codex-, Cursor-, and Claude-specific content belongs in documentation only.
- Never commit a machine-specific game path. Use `ADOFAI_GAME_EXE`, `GameExePath`, or the ignored `ADOFAILoom.local.props`.
- Normal builds may create the installable package in `out/`, but must not deploy or launch the game.
- Deployment requires `DeployMod=true`; launching additionally requires `AutoLaunchGame=true`.

## Source layout

```text
src/
├─ Mod/
│  ├─ ADOFAILoom.Mod.csproj
│  ├─ Info.json
│  ├─ Properties/  assembly metadata
│  ├─ Main.cs       UMM lifecycle coordinator
│  ├─ Bridge/       named-pipe server owned by the game Mod
│  ├─ State/        Unity state capture
│  └─ Threading/    Unity main-thread dispatcher
├─ McpServer/
│  ├─ ADOFAILoom.McpServer.csproj
│  ├─ Program.cs
│  ├─ Bridge/       named-pipe client
│  └─ Tools/        one SDK-discovered class per MCP tool group
└─ Shared/
   └─ Protocol/     DTOs, constants, JSON and pipe framing
```

- Pipe/background code must not reference Unity state directly.
- State capture is the only layer that may read `ADOBase`, `scrController`, `scnGame`, or `scnEditor`.
- Shared protocol code must not depend on Unity, UMM, the MCP SDK, or hosting packages.
- Keep `Main` limited to lifecycle coordination.

## Distribution layout

Every successful root Mod build must recreate this exact installable directory:

```text
out/ADOFAILoom/
├─ ADOFAILoom.dll
├─ ADOFAILoom.McpServer.exe
└─ Info.json
```

- Publish the MCP Server as a Windows x64 self-contained single-file executable.
- The EXE must be inside the Mod folder, next to the Mod DLL.
- Client configuration examples must point to the installed copy under `Mods/ADOFAILoom`, never to `bin`, `obj`, a temporary publish directory, or a developer checkout.
- Do not copy MCP SDK assemblies into the game process; they belong only inside the single-file EXE.

## MCP Server and tool rules

- Use standard MCP stdio transport through `ModelContextProtocol` version `1.4.1`.
- `stdout` is reserved exclusively for MCP protocol messages. Send every log to `stderr`.
- Discover tools through `WithToolsFromAssembly` and official SDK attributes.
- Each tool group belongs in its own file under `src/McpServer/Tools`.
- Public tool name: `get_game_state`.
- Mark read-only tools with `ReadOnly=true`, `Destructive=false`, `Idempotent=true`, and `OpenWorld=false`.
- Return both text content and `StructuredContent` for successful calls.
- When the game or Mod is unavailable, `get_game_state` returns `connected=false`; that is not a tool error.

## Game bridge and threading

- Named pipe: `ADOFAILoom.GameBridge.v1`.
- Start the pipe server when the Mod is enabled and stop it when disabled or unloaded.
- Never access Unity objects from pipe, task-pool, or request threads.
- Enqueue Unity reads through `MainThreadDispatcher` and execute them from UMM `OnUpdate`.
- Support concurrent pipe clients while serializing Unity state reads on the main thread.
- Use one request and response per connection, newline-delimited camelCase JSON, protocol version `1`, a 64 KiB limit, and a 2-second timeout.
- Cancel pending work and dispose listeners, pipes, tasks, and cancellation sources during shutdown.
- Do not add Harmony patches unless stable game APIs cannot implement a required feature; document the reason if that changes.

## Code standards

- Use nullable reference types and cancellation-aware asynchronous APIs.
- Keep classes small and responsibilities explicit.
- Use camelCase JSON and serialization-friendly DTOs.
- Handle expected disconnect and cancellation paths quietly; log unexpected bridge failures through UMM and MCP Server logs through `stderr`.
- Update `README.md` when packaging, protocol, tools, build commands, or client configuration changes.

## Verification without test code

```powershell
dotnet build .\ADOFAILoom.sln -c Release -p:DeployMod=false
dotnet build .\ADOFAILoom.sln -c Release -p:DeployMod=true
```

Then verify:

- `out/ADOFAILoom` contains exactly the Mod DLL, single-file MCP EXE, and `Info.json`.
- Starting the packaged EXE over stdio supports `initialize`, `tools/list`, and `tools/call`, with no stdout log pollution.
- Without the game, `get_game_state` returns `connected=false`.
- With the game and Mod enabled, menu, level select, editor, gameplay, and pause state are correct.
- Disabling or unloading the Mod makes subsequent calls return `connected=false`.

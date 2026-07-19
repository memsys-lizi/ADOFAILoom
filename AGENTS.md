# ADOFAILoom repository rules

## Purpose and architecture

ADOFAILoom is a vendor-neutral MCP server hosted directly inside the A Dance of Fire and Ice Unity process. The Unity Mod Manager Mod exposes a stateless Streamable HTTP endpoint at `http://127.0.0.1:39473/mcp`.

There is one solution, one `netstandard2.1` project, and one distributed DLL. Do not introduce an executable, sidecar process, stdio transport, named-pipe bridge, ASP.NET host, extra project, or extra runtime dependency unless the user explicitly reverses this architecture.

## Non-negotiable rules

- Keep every C# source file under `src/Mod/`.
- Keep the repository root limited to the solution and repository-wide documentation or configuration. Build targets belong under `build/`.
- Never create test projects, test directories, test files, fixtures, mocks, snapshots, or any other automated test code.
- Do not implement fallback behavior. Invalid configuration, protocol data, tool signatures, dependencies, or state reads must fail explicitly.
- Keep the MCP contract client-neutral. Codex-, Cursor-, and Claude-specific configuration belongs in documentation only.
- Never commit a machine-specific game path. Use `ADOFAI_GAME_EXE`, `GameExePath`, or the ignored `ADOFAILoom.local.props`.
- Normal builds may recreate `out/ADOFAILoom`, but must not deploy or launch the game.
- Deployment requires `DeployMod=true`; launching additionally requires `AutoLaunchGame=true`.

## Source layout

```text
src/Mod/
├─ Main.cs       UMM lifecycle coordinator
├─ Actions/      mutating game operations and their result DTOs
├─ Mcp/
│  ├─ Protocol/  MCP and JSON-RPC routing and DTOs
│  ├─ Transport/ loopback Streamable HTTP transport
│  ├─ Tooling/   reflection discovery, schema generation, and invocation framework
│  └─ Tools/     one implementation file per public MCP tool
├─ State/        state DTOs, exact mode mapping, Unity state capture
└─ Threading/    Unity main-thread dispatcher
```

- `Main` coordinates lifecycle only.
- HTTP and protocol code must never reference Unity state directly.
- Only `State` and `Actions` may reference `ADOBase`, `scrController`, `scnGame`, or `scnEditor`. `State` captures read-only snapshots; `Actions` performs explicit mutations.
- Mutating Unity operations belong in `Actions`; MCP tool files delegate to them and do not manipulate scenes or editor objects directly.
- Pure DTOs and mapping logic must not depend on HTTP or UMM.
- Do not add Harmony patches unless a required feature cannot use stable game APIs; document the reason if this changes.

## MCP tools

- Define each tool as a normal method decorated with `[McpTool]` in its own file under `src/Mod/Mcp/Tools/`. Nothing except concrete tool implementations belongs in this directory.
- Tool discovery, input schema generation, parameter binding, annotations, invocation, and result wrapping belong under `src/Mod/Mcp/Tooling/`.
- Tool methods contain business logic only and return serializable DTOs; they must not construct JSON-RPC or MCP response envelopes.
- Tool containers use constructor injection. Every dependency must be registered explicitly in `McpServerFactory`; unresolved dependencies and ambiguous constructors are startup errors.
- Duplicate tool names, missing descriptions, unsupported parameter types, invalid signatures, and non-value return types are startup errors.
- Parameters are required unless explicitly optional. Reject unknown, missing, null, and incorrectly typed arguments.
- `CancellationToken` is injected and excluded from public input schemas.
- Public tools are `get_game_state`, `switch_scene`, `open_level`, `get_editor_state`, `list_visual_events`, the `add/update/remove_camera_move_events` family, the `add/update/remove_filter_events` family, `undo_editor`, `redo_editor`, and `save_level`.
- Mark `get_game_state` read-only, non-destructive, idempotent, and closed-world.
- Mark editor-state and visual-event listing tools read-only, non-destructive, idempotent, and closed-world.
- Mark `switch_scene` and `open_level` mutating, destructive, and non-idempotent. `open_level` is open-world because it reads a user-provided file path.
- Mark visual-event mutations and editor history operations destructive, non-idempotent, and closed-world. Mark `save_level` destructive, idempotent, and open-world.
- Successful calls return both JSON text content and `structuredContent`.

## Unity lifecycle and threading

- Start the HTTP listener when the Mod is enabled and stop it when disabled or unloaded.
- Never access Unity objects from `HttpListener`, task-pool, continuation, or request threads.
- Enqueue game reads through `MainThreadDispatcher` and execute them from UMM `OnUpdate`.
- Preserve the five-second main-thread timeout. Timeout, cancellation, and state-capture failures are explicit tool errors; never return partial or fabricated state.
- Cancel pending work and dispose the listener, active responses, tasks, and cancellation sources during shutdown.

## Streamable HTTP contract

- Bind only to `127.0.0.1:39473`; never bind a LAN interface and never select another port automatically.
- Public endpoint: `http://127.0.0.1:39473/mcp`.
- Use one JSON-RPC message per POST. Return `application/json` for requests and HTTP 202 with no body for accepted notifications.
- GET, DELETE, and other HTTP methods return 405. Do not implement SSE, deprecated HTTP+SSE, or MCP sessions.
- Validate every non-empty `Origin` and allow loopback origins only.
- Require JSON UTF-8 content, both MCP Accept media types, a supported protocol version after initialization, and a maximum 64 KiB body.
- Supported versions are `2025-03-26`, `2025-06-18`, and `2025-11-25`.
- Support `initialize`, `ping`, `notifications/initialized`, `tools/list`, and `tools/call`.
- Use JSON-RPC errors for malformed requests, unsupported methods, and invalid parameters. Use `isError=true` tool results only for failures after valid tool invocation begins.
- Catch exceptions only at transport and invocation boundaries. Expected shutdown/disconnect exceptions may be quiet; every unexpected exception must be logged in full and must stop or fail the affected operation.

## State contract

Current schema version is `2`:

```json
{
  "schemaVersion": 2,
  "scene": "scnEditor",
  "mode": "editor",
  "paused": true
}
```

- Do not add a `connected` field. If the game or Mod is unavailable, the HTTP endpoint is unavailable.
- `mode` is `menu | level_select | editor | gameplay | unknown`.
- Map only explicitly recognized menu scenes to `menu`; every unrecognized scene retains its real name and maps to `unknown`.
- `paused=null` means no `scrController` exists; do not turn it into `false`.
- Additive fields may retain schema version 2. Breaking changes require a new version and an explicit migration decision.

## Mutating action contracts

- `switch_scene(sceneName)` accepts an exact loadable Unity scene name. Do not add aliases, spelling correction, trimming, or alternate-scene fallback.
- Validate built-in scenes through Unity and installed DLC scenes through the game's DLC registry before starting a transition.
- `open_level(levelPath)` accepts only an absolute path to an existing `.adofai` file and requires the current scene to be `scnEditor`. It must not switch scenes automatically or reinterpret the value as an official level ID.
- Both actions must use the game's unsaved-editor confirmation flow. Return `started` when the action starts synchronously and `awaiting_confirmation` when the game is waiting for the user.
- Editor visual tools use native `.adofai` units: zero-based floor, degree `angleOffset`, beat `duration`, tile-unit position, and percentage zoom or intensity. Do not convert from seconds, normalize percentages, or infer timing from audio.
- Never expose arbitrary `LevelData` paths or property dictionaries for mutation. Public visual mutations use strict typed DTOs and game runtime metadata for validation.
- Every editor mutation requires the exact current level revision. Event updates and removals additionally require an exact collection/index/type/fingerprint reference; stale references fail without searching for a replacement.
- Validate the entire batch before mutation and use one `SaveStateScope` per successful batch. Preserve request order and refresh affected event indicators and floors through the editor APIs.
- Do not call `PropertyInfo.Validate` for MCP input because it clamps. Reject values outside the runtime metadata range, invalid enum names, unsupported first-floor placement, locked events, preview mode, path lock, empty batches, and duplicate references.
- Camera tools support only `MoveCamera`. Filter tools support only standard `SetFilter`; do not accept `SetFilterAdvanced` property dictionaries.
- Visual mutations remain in editor memory until `save_level` is called. `save_level` must reject missing/non-`.adofai` paths and verify the written JSON revision before reporting success.

## Build and manual verification

```powershell
dotnet build -c Release -p:DeployMod=false
dotnet build -c Release -p:DeployMod=true
```

- Builds must complete with zero errors and zero warnings.
- `out/ADOFAILoom` must contain only `ADOFAILoom.dll` and `Info.json`.
- Deployment must recreate `Mods/ADOFAILoom` so obsolete files cannot survive an upgrade.
- Verify protocol behavior with direct HTTP requests, MCP Inspector, UMM logs, and manual in-game checks. Never create automated test code.
- Manually cover initialize, initialized notification, tools/list, all public tools, malformed requests, invalid headers, scene switching, editor confirmation, valid/invalid level paths, editor revisions and stale references, camera/filter CRUD, undo/redo/save, menu, level select, custom level select, editor, gameplay, pause, disable, and shutdown.
- Update `README.md` whenever the endpoint, port, versions, tool contract, build behavior, or client configuration changes.

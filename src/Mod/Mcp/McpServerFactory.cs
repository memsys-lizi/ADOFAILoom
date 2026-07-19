using System;
using System.Collections.Generic;
using ADOFAILoom.Actions;
using ADOFAILoom.Actions.EditorPreview;
using ADOFAILoom.Actions.EditorWorkflow;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Resources;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.Mcp.Transport;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp
{
    internal static class McpServerFactory
    {
        public static McpServer Create(
            MainThreadDispatcher dispatcher,
            McpToolAvailability toolAvailability,
            Action<string> log
        )
        {
            var sceneSwitcher = new SceneSwitcher(dispatcher);
            var levelOpener = new LevelOpener(dispatcher);
            var visualEventReader = new VisualEventReader(dispatcher);
            var visualEventMutationEngine = new VisualEventMutationEngine(dispatcher);
            var decorationMutationEngine = new DecorationMutationEngine(dispatcher);
            var cameraMoveEventActions = new CameraMoveEventActions(visualEventMutationEngine);
            var filterEventActions = new FilterEventActions(visualEventMutationEngine);
            var screenEffectActions = new ScreenEffectActions(visualEventMutationEngine);
            var backgroundEventActions = new BackgroundEventActions(visualEventMutationEngine);
            var trackEventActions = new TrackEventActions(visualEventMutationEngine);
            var textEventActions = new TextEventActions(
                decorationMutationEngine,
                visualEventMutationEngine
            );
            var imageDecorationActions = new ImageDecorationActions(decorationMutationEngine);
            var moveDecorationEventActions = new MoveDecorationEventActions(
                visualEventMutationEngine
            );
            var defaultTextEventActions = new DefaultTextEventActions(visualEventMutationEngine);
            var objectDecorationActions = new ObjectDecorationActions(decorationMutationEngine);
            var setObjectEventActions = new SetObjectEventActions(visualEventMutationEngine);
            var emitParticleEventActions = new EmitParticleEventActions(visualEventMutationEngine);
            var particleDecorationActions = new ParticleDecorationActions(decorationMutationEngine);
            var setParticleEventActions = new SetParticleEventActions(visualEventMutationEngine);
            var editorWorkflow = new EditorWorkflowActions(dispatcher);
            var editorPreview = new EditorPreviewActions(dispatcher);
            var services = new Dictionary<Type, object>
            {
                [typeof(MainThreadDispatcher)] = dispatcher,
                [typeof(SceneSwitcher)] = sceneSwitcher,
                [typeof(LevelOpener)] = levelOpener,
                [typeof(VisualEventReader)] = visualEventReader,
                [typeof(CameraMoveEventActions)] = cameraMoveEventActions,
                [typeof(FilterEventActions)] = filterEventActions,
                [typeof(ScreenEffectActions)] = screenEffectActions,
                [typeof(BackgroundEventActions)] = backgroundEventActions,
                [typeof(TrackEventActions)] = trackEventActions,
                [typeof(TextEventActions)] = textEventActions,
                [typeof(ImageDecorationActions)] = imageDecorationActions,
                [typeof(MoveDecorationEventActions)] = moveDecorationEventActions,
                [typeof(DefaultTextEventActions)] = defaultTextEventActions,
                [typeof(ObjectDecorationActions)] = objectDecorationActions,
                [typeof(SetObjectEventActions)] = setObjectEventActions,
                [typeof(EmitParticleEventActions)] = emitParticleEventActions,
                [typeof(ParticleDecorationActions)] = particleDecorationActions,
                [typeof(SetParticleEventActions)] = setParticleEventActions,
                [typeof(EditorWorkflowActions)] = editorWorkflow,
                [typeof(EditorPreviewActions)] = editorPreview,
            };
            McpToolRegistry tools = McpToolRegistry.Discover(
                typeof(McpServerFactory).Assembly,
                services,
                toolAvailability,
                McpProtocol.JsonOptions
            );
            var resources = new EmbeddedGuideCatalog(typeof(McpServerFactory).Assembly);
            var router = new McpRequestRouter(tools, resources, log);
            var transport = new StreamableHttpTransport(router, log);
            return new McpServer(transport);
        }
    }
}

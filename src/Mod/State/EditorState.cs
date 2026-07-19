using System.Collections.Generic;

namespace ADOFAILoom.State
{
    internal sealed class EditorState
    {
        public EditorState(
            string? levelPath,
            int floorCount,
            IReadOnlyList<int> selectedFloors,
            IReadOnlyList<int> selectedDecorations,
            int? currentFloor,
            bool previewing,
            bool canUndo,
            bool canRedo,
            string revision)
        {
            LevelPath = levelPath;
            FloorCount = floorCount;
            SelectedFloors = selectedFloors;
            SelectedDecorations = selectedDecorations;
            CurrentFloor = currentFloor;
            Previewing = previewing;
            CanUndo = canUndo;
            CanRedo = canRedo;
            Revision = revision;
        }

        public string? LevelPath { get; }

        public int FloorCount { get; }

        public IReadOnlyList<int> SelectedFloors { get; }

        public IReadOnlyList<int> SelectedDecorations { get; }

        public int? CurrentFloor { get; }

        public bool Previewing { get; }

        public bool CanUndo { get; }

        public bool CanRedo { get; }

        public string Revision { get; }
    }
}

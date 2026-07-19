using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class CameraMoveEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public CameraMoveEventActions(VisualEventMutationEngine engine)
        {
            this.engine = engine;
        }

        public Task<VisualEventMutationResult> AddAsync(
            string expectedRevision,
            CameraMoveCreate[] events,
            CancellationToken cancellationToken
        )
        {
            return engine.AddAsync(expectedRevision, events, Create, cancellationToken);
        }

        public Task<VisualEventMutationResult> UpdateAsync(
            string expectedRevision,
            CameraMoveUpdate[] events,
            CancellationToken cancellationToken
        )
        {
            return engine.UpdateAsync(
                expectedRevision,
                events,
                LevelEventType.MoveCamera,
                input => input.Reference,
                ApplyUpdate,
                cancellationToken
            );
        }

        public Task<VisualEventMutationResult> RemoveAsync(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        )
        {
            return engine.RemoveAsync(
                expectedRevision,
                eventRefs,
                LevelEventType.MoveCamera,
                cancellationToken
            );
        }

        private static LevelEvent Create(scnEditor editor, CameraMoveCreate input)
        {
            var levelEvent = new LevelEvent(input.Floor, LevelEventType.MoveCamera);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset);
            VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration);
            VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease);

            int targets = 0;
            targets += SetOptional(levelEvent, "position", input.Position);
            targets += SetOptional(levelEvent, "rotation", input.Rotation);
            targets += SetOptional(levelEvent, "zoom", input.Zoom);
            targets += SetOptional(levelEvent, "relativeTo", input.RelativeTo);
            if (targets == 0)
            {
                throw new ArgumentException(
                    "A camera move must set at least one of position, rotation, zoom, or relativeTo."
                );
            }

            return levelEvent;
        }

        private static LevelEvent ApplyUpdate(
            scnEditor editor,
            LevelEvent levelEvent,
            CameraMoveUpdate input
        )
        {
            var disabled = new HashSet<CameraMoveProperty>();
            if (input.DisabledProperties != null)
            {
                foreach (CameraMoveProperty property in input.DisabledProperties)
                {
                    if (!disabled.Add(property))
                    {
                        throw new ArgumentException(
                            $"Camera property '{property}' was disabled more than once."
                        );
                    }
                }
            }

            RejectConflict(input.Position != null, disabled, CameraMoveProperty.Position);
            RejectConflict(input.Rotation.HasValue, disabled, CameraMoveProperty.Rotation);
            RejectConflict(input.Zoom.HasValue, disabled, CameraMoveProperty.Zoom);
            RejectConflict(input.RelativeTo.HasValue, disabled, CameraMoveProperty.RelativeTo);

            bool changed = disabled.Count > 0;
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor.Value);
                levelEvent.floor = input.Floor.Value;
                changed = true;
            }

            if (input.AngleOffset.HasValue)
            {
                VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset.Value);
                changed = true;
            }

            if (input.Duration.HasValue)
            {
                VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration.Value);
                changed = true;
            }

            if (input.Ease.HasValue)
            {
                VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease.Value);
                changed = true;
            }

            changed |= UpdateOptional(levelEvent, "position", input.Position);
            changed |= UpdateOptional(levelEvent, "rotation", input.Rotation);
            changed |= UpdateOptional(levelEvent, "zoom", input.Zoom);
            changed |= UpdateOptional(levelEvent, "relativeTo", input.RelativeTo);
            foreach (CameraMoveProperty property in disabled)
            {
                VisualEventValidator.Disable(levelEvent, PropertyName(property));
            }

            if (!changed)
            {
                throw new ArgumentException("Camera move update contains no changes.");
            }

            return levelEvent;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, VisualVector2? value)
        {
            if (value == null)
            {
                VisualEventValidator.Disable(levelEvent, key);
                return 0;
            }

            VisualEventValidator.SetVector2(levelEvent, key, value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, float? value)
        {
            if (!value.HasValue)
            {
                VisualEventValidator.Disable(levelEvent, key);
                return 0;
            }

            VisualEventValidator.SetFloat(levelEvent, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, CameraRelativeTo? value)
        {
            if (!value.HasValue)
            {
                VisualEventValidator.Disable(levelEvent, key);
                return 0;
            }

            VisualEventValidator.SetEnum(levelEvent, key, value.Value);
            return 1;
        }

        private static bool UpdateOptional(LevelEvent levelEvent, string key, VisualVector2? value)
        {
            if (value == null)
            {
                return false;
            }

            VisualEventValidator.SetVector2(levelEvent, key, value);
            return true;
        }

        private static bool UpdateOptional(LevelEvent levelEvent, string key, float? value)
        {
            if (!value.HasValue)
            {
                return false;
            }

            VisualEventValidator.SetFloat(levelEvent, key, value.Value);
            return true;
        }

        private static bool UpdateOptional(
            LevelEvent levelEvent,
            string key,
            CameraRelativeTo? value
        )
        {
            if (!value.HasValue)
            {
                return false;
            }

            VisualEventValidator.SetEnum(levelEvent, key, value.Value);
            return true;
        }

        private static string PropertyName(CameraMoveProperty property)
        {
            return property switch
            {
                CameraMoveProperty.Position => "position",
                CameraMoveProperty.Rotation => "rotation",
                CameraMoveProperty.Zoom => "zoom",
                CameraMoveProperty.RelativeTo => "relativeTo",
                _ => throw new ArgumentOutOfRangeException(nameof(property)),
            };
        }

        private static void RejectConflict(
            bool hasValue,
            ISet<CameraMoveProperty> disabled,
            CameraMoveProperty property
        )
        {
            if (hasValue && disabled.Contains(property))
            {
                throw new ArgumentException(
                    $"Camera property '{property}' cannot be updated and disabled in the same request."
                );
            }
        }
    }
}

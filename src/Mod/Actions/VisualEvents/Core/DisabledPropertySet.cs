using System;
using System.Collections.Generic;
using ADOFAI;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class DisabledPropertySet<TProperty>
        where TProperty : struct, Enum
    {
        private readonly HashSet<TProperty> properties;

        public DisabledPropertySet(IEnumerable<TProperty>? properties)
        {
            this.properties = new HashSet<TProperty>();
            if (properties == null)
                return;

            foreach (TProperty property in properties)
            {
                if (!this.properties.Add(property))
                    throw new ArgumentException(
                        $"Property '{property}' was disabled more than once."
                    );
            }
        }

        public void Apply(
            LevelEvent levelEvent,
            TProperty property,
            bool hasUpdateValue,
            string gamePropertyName
        )
        {
            if (!properties.Contains(property))
                return;

            if (hasUpdateValue)
                throw new ArgumentException(
                    $"Property '{property}' cannot be updated and disabled together."
                );

            VisualEventValidator.Disable(levelEvent, gamePropertyName);
        }
    }
}

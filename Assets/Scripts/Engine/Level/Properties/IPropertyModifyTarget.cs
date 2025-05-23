using System.Collections.Generic;
using PVZEngine.Modifiers;

namespace PVZEngine.Level
{
    public interface IPropertyModifyTarget
    {
        bool GetFallbackProperty(PropertyKey name, out object value);
        void GetModifierItems(PropertyKey name, List<ModifierContainerItem> results);
        void UpdateModifiedProperty(PropertyKey name, object beforeValue, object afterValue);
        PropertyModifier[] GetModifiersUsingProperty(PropertyKey name);
        IEnumerable<PropertyKey> GetModifiedProperties();
    }
}

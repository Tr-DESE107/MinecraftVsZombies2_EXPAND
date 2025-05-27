using System.Collections.Generic;
using PVZEngine.Modifiers;

namespace PVZEngine.Level
{
    public interface IPropertyModifyTarget
    {
        bool GetFallbackProperty<T>(PropertyKey<T> name, out T value);
        void GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results);
        void UpdateModifiedProperty<T>(PropertyKey<T> name, T beforeValue, T afterValue);
        PropertyModifier[] GetModifiersUsingProperty(IPropertyKey name);
        IEnumerable<IPropertyKey> GetModifiedProperties();
    }
}

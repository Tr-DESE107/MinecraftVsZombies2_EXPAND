﻿using System.Collections.Generic;
using PVZEngine.Modifiers;

namespace PVZEngine.Level
{
    public interface IPropertyModifyTarget
    {
        bool GetFallbackProperty(IPropertyKey name, out object value);
        void GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results);
        void UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue);
        PropertyModifier[] GetModifiersUsingProperty(IPropertyKey name);
        IEnumerable<IPropertyKey> GetModifiedProperties();
    }
}

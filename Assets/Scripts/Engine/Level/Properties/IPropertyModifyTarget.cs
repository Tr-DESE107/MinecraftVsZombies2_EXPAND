using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Base;
using PVZEngine.Modifiers;
using Tools;

namespace PVZEngine.Level
{
    public interface IPropertyModifyTarget
    {
        bool GetFallbackProperty(PropertyKey name, out object value);
        void GetModifierItems(PropertyKey name, List<ModifierContainerItem> results);
        void UpdateModifiedProperty(PropertyKey name, object value);
        PropertyModifier[] GetModifiersUsingProperty(PropertyKey name);
        IEnumerable<PropertyKey> GetModifiedProperties();
    }
}

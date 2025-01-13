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
        bool GetFallbackProperty(string name, out object value);
        void GetModifierItems(string name, List<ModifierContainerItem> results);
        void UpdateModifiedProperty(string name, object value);
        PropertyModifier[] GetModifiersUsingProperty(string name);
        IEnumerable<string> GetModifiedProperties();
    }
}

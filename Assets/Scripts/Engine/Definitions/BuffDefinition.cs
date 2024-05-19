using System.Collections.Generic;

namespace PVZEngine
{
    public abstract class BuffDefinition : Definition
    {
        public PropertyModifier[] GetModifiers()
        {
            return modifiers.ToArray();
        }
        protected void AddModifier(PropertyModifier modifier)
        {
            modifiers.Add(modifier);
        }
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
    }
}

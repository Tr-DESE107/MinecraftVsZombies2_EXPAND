using System.Collections.Generic;

namespace PVZEngine
{
    public abstract class BuffDefinition : Definition
    {
        public BuffDefinition(string nsp, string name) : base(nsp, name)
        {
        }
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

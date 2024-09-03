using System.Collections.Generic;
using PVZEngine.LevelManaging;

namespace PVZEngine.Definitions
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

using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Level;
using PVZEngine.Modifiers;

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
        public virtual void PostAdd(Buff buff) { }
        public virtual void PostRemove(Buff buff) { }
        public virtual void PostUpdate(Buff buff) { }
        protected void AddModifier(PropertyModifier modifier)
        {
            modifiers.Add(modifier);
        }
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
    }
}

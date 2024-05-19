using System.Collections.Generic;

namespace PVZEngine
{
    public abstract class BuffDefinition : Definition
    {
        public Modifier[] GetModifiers()
        {
            return modifiers.ToArray();
        }
        private List<Modifier> modifiers;
    }
}

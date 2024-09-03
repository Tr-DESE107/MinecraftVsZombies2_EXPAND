using PVZEngine;
using PVZEngine.LevelManaging;

namespace MVZ2.GameContent.Modifiers
{
    public class NamespaceIDModifier : PropertyModifier<NamespaceID>
    {
        public NamespaceIDModifier(string propertyName, ModifyOperator op, NamespaceID valueConst) : base(propertyName, op, valueConst)
        {
        }

        public NamespaceIDModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override NamespaceID CalculatePropertyGeneric(Buff buff, NamespaceID value)
        {
            return GetValueGeneric(buff);
        }
    }
}

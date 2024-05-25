using PVZEngine;

namespace MVZ2.GameContent.Modifiers
{
    public class BooleanModifier : PropertyModifier<bool>
    {
        public BooleanModifier(string propertyName, ModifyOperator op, bool valueConst) : base(propertyName, op, valueConst)
        {
        }

        public BooleanModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override bool CalculatePropertyGeneric(Buff buff, bool value)
        {
            return GetValueGeneric(buff);
        }
    }
}

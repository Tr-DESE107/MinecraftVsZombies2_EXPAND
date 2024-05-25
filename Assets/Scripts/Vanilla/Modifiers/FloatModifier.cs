using PVZEngine;

namespace MVZ2.GameContent.Modifiers
{
    public class FloatModifier : PropertyModifier<float>
    {
        public FloatModifier(string propertyName, ModifyOperator op, float valueConst) : base(propertyName, op, valueConst)
        {
        }

        public FloatModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override float CalculatePropertyGeneric(Buff buff, float value)
        {
            switch (Operator)
            {
                case ModifyOperator.Add:
                    return value + GetValueGeneric(buff);
                case ModifyOperator.Multiply:
                    return value * GetValueGeneric(buff);
                case ModifyOperator.Average:
                    return (value + GetValueGeneric(buff)) / 2;
                case ModifyOperator.Set:
                    return GetValueGeneric(buff);
            }
            return value;
        }
    }
}

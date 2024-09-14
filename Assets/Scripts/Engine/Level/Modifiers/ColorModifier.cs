using PVZEngine.LevelManagement;
using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class ColorModifier : PropertyModifier<Color>
    {
        public ColorModifier(string propertyName, ModifyOperator op, Color valueConst) : base(propertyName, op, valueConst)
        {
        }

        public ColorModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override Color CalculatePropertyGeneric(Buff buff, Color value)
        {
            switch (Operator)
            {
                case ModifyOperator.Add:
                    return value + GetValueGeneric(buff);
                case ModifyOperator.Multiply:
                    return value * GetValueGeneric(buff);
                case ModifyOperator.Average:
                    return (value + GetValueGeneric(buff)) * 0.5f;
                case ModifyOperator.Set:
                    return GetValueGeneric(buff);
            }
            return value;
        }
    }
}

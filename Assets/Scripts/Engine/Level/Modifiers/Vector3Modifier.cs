using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class Vector3Modifier : PropertyModifier<Vector3>
    {
        public Vector3Modifier(string propertyName, ModifyOperator op, Vector3 valueConst) : base(propertyName, op, valueConst)
        {
        }

        public Vector3Modifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override Vector3 CalculatePropertyGeneric(Buff buff, Vector3 value)
        {
            switch (Operator)
            {
                case ModifyOperator.Add:
                    return value + GetValueGeneric(buff);
                case ModifyOperator.Multiply:
                    return Vector3.Scale(value, GetValueGeneric(buff));
                case ModifyOperator.Average:
                    return (value + GetValueGeneric(buff)) * 0.5f;
                case ModifyOperator.Set:
                    return GetValueGeneric(buff);
            }
            return value;
        }
    }
}

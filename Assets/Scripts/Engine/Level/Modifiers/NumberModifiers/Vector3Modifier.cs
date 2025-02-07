using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class Vector3Modifier : NumberModifier<Vector3>
    {
        public Vector3Modifier(PropertyKey propertyName, NumberOperator op, Vector3 valueConst) : base(propertyName, op, valueConst)
        {
        }

        public Vector3Modifier(PropertyKey propertyName, NumberOperator op, PropertyKey buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.vector3Calculator;
        }
    }
}

using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class Vector3Modifier : NumberModifier<Vector3>
    {
        public Vector3Modifier(string propertyName, NumberOperator op, Vector3 valueConst) : base(propertyName, op, valueConst)
        {
        }

        public Vector3Modifier(string propertyName, NumberOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
    }
}

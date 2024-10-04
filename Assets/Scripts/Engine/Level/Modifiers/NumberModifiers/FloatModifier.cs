using PVZEngine.Level;

namespace PVZEngine.Modifiers
{
    public class FloatModifier : NumberModifier<float>
    {
        public FloatModifier(string propertyName, NumberOperator op, float valueConst) : base(propertyName, op, valueConst)
        {
        }

        public FloatModifier(string propertyName, NumberOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
    }
}

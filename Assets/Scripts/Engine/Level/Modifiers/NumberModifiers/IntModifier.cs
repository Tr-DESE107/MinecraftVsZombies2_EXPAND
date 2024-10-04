using PVZEngine.Level;

namespace PVZEngine.Modifiers
{
    public class IntModifier : NumberModifier<int>
    {
        public IntModifier(string propertyName, NumberOperator op, int valueConst) : base(propertyName, op, valueConst)
        {
        }

        public IntModifier(string propertyName, NumberOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
    }
}

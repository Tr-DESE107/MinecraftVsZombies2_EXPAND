namespace PVZEngine.Modifiers
{
    public class IntModifier : NumberModifier<int>
    {
        public IntModifier(PropertyKey propertyName, NumberOperator op, int valueConst) : base(propertyName, op, valueConst)
        {
        }

        public IntModifier(PropertyKey propertyName, NumberOperator op, PropertyKey buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.intCalculator;
        }
    }
}

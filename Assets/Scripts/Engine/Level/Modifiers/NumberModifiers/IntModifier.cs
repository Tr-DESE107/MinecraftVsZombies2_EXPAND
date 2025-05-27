namespace PVZEngine.Modifiers
{
    public class IntModifier : NumberModifier<int>
    {
        public IntModifier(PropertyKey<int> propertyName, NumberOperator op, int valueConst, int priority = 0) : base(propertyName, op, valueConst, priority)
        {
        }

        public IntModifier(PropertyKey<int> propertyName, NumberOperator op, PropertyKey<int> buffPropertyName, int priority = 0) : base(propertyName, op, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.intCalculator;
        }
    }
}

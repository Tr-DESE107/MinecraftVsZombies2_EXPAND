namespace PVZEngine.Modifiers
{
    public class FloatModifier : NumberModifier<float>
    {
        public FloatModifier(PropertyKey<float> propertyName, NumberOperator op, float valueConst, int priority = 0) : base(propertyName, op, valueConst, priority)
        {
        }

        public FloatModifier(PropertyKey<float> propertyName, NumberOperator op, PropertyKey<float> buffPropertyName, int priority = 0) : base(propertyName, op, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.floatCalculator;
        }
    }
}

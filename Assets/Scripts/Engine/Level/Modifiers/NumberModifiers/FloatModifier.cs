namespace PVZEngine.Modifiers
{
    public class FloatModifier : NumberModifier<float>
    {
        public FloatModifier(PropertyKey propertyName, NumberOperator op, float valueConst) : base(propertyName, op, valueConst)
        {
        }

        public FloatModifier(PropertyKey propertyName, NumberOperator op, PropertyKey buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.floatCalculator;
        }
    }
}

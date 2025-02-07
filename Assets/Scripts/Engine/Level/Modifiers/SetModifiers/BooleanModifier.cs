namespace PVZEngine.Modifiers
{
    public class BooleanModifier : PropertyModifier<bool>
    {
        public BooleanModifier(PropertyKey propertyName, bool valueConst) : base(propertyName, valueConst)
        {
        }

        public BooleanModifier(PropertyKey propertyName, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.booleanCalculator;
        }
    }
}

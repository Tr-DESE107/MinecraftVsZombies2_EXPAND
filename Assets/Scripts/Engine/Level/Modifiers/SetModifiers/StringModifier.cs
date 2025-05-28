namespace PVZEngine.Modifiers
{
    public class StringModifier : PropertyModifier<string>
    {
        public StringModifier(PropertyKey<string> propertyName, string valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
        }

        public StringModifier(PropertyKey<string> propertyName, PropertyKey<string> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.stringCalculator;
        }
    }
}

namespace PVZEngine.Modifiers
{
    public class StringModifier : PropertyModifier<string>
    {
        public StringModifier(PropertyKey propertyName, string valueConst) : base(propertyName, null)
        {
            ConstValue = valueConst;
        }
        public static StringModifier FromPropertyName(PropertyKey propertyName, PropertyKey buffPropertyName)
        {
            return new StringModifier(propertyName, null)
            {
                UsingContainerPropertyName = buffPropertyName
            };
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.stringCalculator;
        }
    }
}

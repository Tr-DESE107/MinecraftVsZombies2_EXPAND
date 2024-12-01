namespace PVZEngine.Modifiers
{
    public class BooleanModifier : PropertyModifier<bool>
    {
        public BooleanModifier(string propertyName, bool valueConst) : base(propertyName, valueConst)
        {
        }

        public BooleanModifier(string propertyName, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.booleanCalculator;
        }
    }
}

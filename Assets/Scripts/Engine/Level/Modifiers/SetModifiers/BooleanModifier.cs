namespace PVZEngine.Modifiers
{
    public class BooleanModifier : PropertyModifier<bool>
    {
        public BooleanModifier(PropertyKey<bool> propertyName, bool valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
        }

        public BooleanModifier(PropertyKey<bool> propertyName, PropertyKey<bool> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.booleanCalculator;
        }
    }
}

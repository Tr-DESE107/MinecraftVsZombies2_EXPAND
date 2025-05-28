namespace PVZEngine.Modifiers
{
    public class NamespaceIDModifier : PropertyModifier<NamespaceID>
    {
        public NamespaceIDModifier(PropertyKey<NamespaceID> propertyName, NamespaceID valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
        }

        public NamespaceIDModifier(PropertyKey<NamespaceID> propertyName, PropertyKey<NamespaceID> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.namespaceIDCalculator;
        }
    }
}

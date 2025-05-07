namespace PVZEngine.Modifiers
{
    public class NamespaceIDArrayModifier : PropertyModifier<NamespaceID[]>
    {
        public NamespaceIDArrayModifier(PropertyKey propertyName, NamespaceID[] valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
        }

        public NamespaceIDArrayModifier(PropertyKey propertyName, PropertyKey buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.namespaceIDArrayCalculator;
        }
    }
}

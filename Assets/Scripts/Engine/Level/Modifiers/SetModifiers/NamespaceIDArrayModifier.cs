namespace PVZEngine.Modifiers
{
    public class NamespaceIDArrayModifier : PropertyModifier<NamespaceID[]>
    {
        public NamespaceIDArrayModifier(PropertyKey<NamespaceID[]> propertyName, NamespaceID[] valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
        }

        public NamespaceIDArrayModifier(PropertyKey<NamespaceID[]> propertyName, PropertyKey<NamespaceID[]> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.namespaceIDArrayCalculator;
        }
    }
}

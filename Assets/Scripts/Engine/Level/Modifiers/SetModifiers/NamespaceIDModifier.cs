namespace PVZEngine.Modifiers
{
    public class NamespaceIDModifier : PropertyModifier<NamespaceID>
    {
        public NamespaceIDModifier(PropertyKey propertyName, NamespaceID valueConst) : base(propertyName, valueConst)
        {
        }

        public NamespaceIDModifier(PropertyKey propertyName, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.namespaceIDCalculator;
        }
    }
}

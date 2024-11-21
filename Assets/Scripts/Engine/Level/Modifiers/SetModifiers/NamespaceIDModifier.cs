namespace PVZEngine.Modifiers
{
    public class NamespaceIDModifier : PropertyModifier<NamespaceID>
    {
        public NamespaceIDModifier(string propertyName, NamespaceID valueConst) : base(propertyName, valueConst)
        {
        }

        public NamespaceIDModifier(string propertyName, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
    }
}

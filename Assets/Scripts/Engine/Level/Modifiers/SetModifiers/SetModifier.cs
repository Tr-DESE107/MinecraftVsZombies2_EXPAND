namespace PVZEngine.Modifiers
{
    public abstract class SetModifier<T> : PropertyModifier<T>
    {
        protected SetModifier(PropertyKey propertyName, T constValue) : base(propertyName, constValue)
        {
        }

        protected SetModifier(PropertyKey propertyName, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
    }
}

namespace PVZEngine.Modifiers
{
    public abstract class SetModifier<T> : PropertyModifier<T>
    {
        protected SetModifier(string propertyName, T constValue) : base(propertyName, constValue)
        {
        }

        protected SetModifier(string propertyName, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
    }
}

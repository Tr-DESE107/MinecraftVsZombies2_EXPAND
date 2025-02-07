namespace PVZEngine.Modifiers
{
    public abstract class NumberModifier<T> : PropertyModifier<T>
    {
        protected NumberModifier(PropertyKey propertyName, NumberOperator op, T constValue) : base(propertyName, constValue)
        {
            Operator = op;
        }

        protected NumberModifier(PropertyKey propertyName, NumberOperator op, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
            Operator = op;
        }

        public NumberOperator Operator { get; private set; }
    }
}

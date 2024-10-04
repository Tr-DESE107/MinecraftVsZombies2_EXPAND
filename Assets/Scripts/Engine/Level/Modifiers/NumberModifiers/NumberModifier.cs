namespace PVZEngine.Modifiers
{
    public abstract class NumberModifier<T> : PropertyModifier<T>
    {
        protected NumberModifier(string propertyName, NumberOperator op, T constValue) : base(propertyName, constValue)
        {
            Operator = op;
        }

        protected NumberModifier(string propertyName, NumberOperator op, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
            Operator = op;
        }

        public NumberOperator Operator { get; private set; }
    }
}

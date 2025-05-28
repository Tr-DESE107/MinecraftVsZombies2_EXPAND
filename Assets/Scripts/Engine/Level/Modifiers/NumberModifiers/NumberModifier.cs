﻿namespace PVZEngine.Modifiers
{
    public abstract class NumberModifier<T> : PropertyModifier<T>
    {
        protected NumberModifier(PropertyKey<T> propertyName, NumberOperator op, T constValue, int priority) : base(propertyName, constValue, priority)
        {
            Operator = op;
        }

        protected NumberModifier(PropertyKey<T> propertyName, NumberOperator op, PropertyKey<T> buffPropertyName, int priority) : base(propertyName, buffPropertyName, priority)
        {
            Operator = op;
        }

        public NumberOperator Operator { get; private set; }
    }
}

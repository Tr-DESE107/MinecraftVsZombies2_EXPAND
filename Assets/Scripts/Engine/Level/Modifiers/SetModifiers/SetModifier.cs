﻿namespace PVZEngine.Modifiers
{
    public abstract class SetModifier<T> : PropertyModifier<T>
    {
        protected SetModifier(PropertyKey<T> propertyName, T constValue, int priority = 0) : base(propertyName, constValue, priority)
        {
        }

        protected SetModifier(PropertyKey<T> propertyName, PropertyKey<T> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
        }
    }
}

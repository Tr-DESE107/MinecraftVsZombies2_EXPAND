using PVZEngine;

namespace MVZ2.Vanilla.Modifiers
{
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(string propertyName, ModifyOperator op, T valueConst) : base(propertyName, op, valueConst)
        {
        }

        protected PropertyModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override sealed object CalculateProperty(Entity entity, Buff buff, object value)
        {
            if (value is T tValue)
                return CalculatePropertyGeneric(entity, buff, tValue);
            return value;
        }
        public T GetValueGeneric(Buff buff)
        {
            if (GetValue(buff) is T tValue)
                return tValue;
            return default;
        }
        public abstract T CalculatePropertyGeneric(Entity entity, Buff buff, T value);
    }
    public class IntModifier : PropertyModifier<int>
    {
        public IntModifier(string propertyName, ModifyOperator op, int valueConst) : base(propertyName, op, valueConst)
        {
        }

        public IntModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override int CalculatePropertyGeneric(Entity entity, Buff buff, int value)
        {
            switch (Operator)
            {
                case ModifyOperator.Add:
                    return value + GetValueGeneric(buff);
                case ModifyOperator.Multiply:
                    return value * GetValueGeneric(buff);
                case ModifyOperator.Set:
                    return GetValueGeneric(buff);
            }
            return value;
        }
    }
}

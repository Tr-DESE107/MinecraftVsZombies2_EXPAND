namespace PVZEngine
{
    public abstract class PropertyModifier
    {
        public PropertyModifier(string propertyName, ModifyOperator op, object valueConst)
        {
            PropertyName = propertyName;
            Operator = op;
            ConstValue = valueConst;
        }
        public PropertyModifier(string propertyName, ModifyOperator op, string buffPropertyName)
        {
            PropertyName = propertyName;
            Operator = op;
            UsingBuffPropertyName = buffPropertyName;
        }
        public virtual void PostAdd(Buff buff)
        {

        }
        public virtual void PostRemove(Buff buff)
        {

        }
        public object GetValue(Buff buff)
        {
            if (!string.IsNullOrEmpty(UsingBuffPropertyName))
            {
                return buff.GetProperty<object>(UsingBuffPropertyName);
            }
            else
            {
                return ConstValue;
            }
        }
        public abstract object CalculateProperty(Buff buff, object value);
        public string PropertyName { get; set; }
        public object ConstValue { get; set; }
        public string UsingBuffPropertyName { get; set; }
        public ModifyOperator Operator { get; set; }
    }
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(string propertyName, ModifyOperator op, T valueConst) : base(propertyName, op, valueConst)
        {
        }

        protected PropertyModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override sealed object CalculateProperty(Buff buff, object value)
        {
            if (value == null)
                value = default(T);
            if (value is T tValue)
                return CalculatePropertyGeneric(buff, tValue);
            return value;
        }
        public T GetValueGeneric(Buff buff)
        {
            if (GetValue(buff) is T tValue)
                return tValue;
            return default;
        }
        public abstract T CalculatePropertyGeneric(Buff buff, T value);
    }
}

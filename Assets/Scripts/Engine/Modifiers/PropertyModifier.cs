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
        public virtual void PostAdd(Entity entity, Buff buff)
        {

        }
        public virtual void PostRemove(Entity entity, Buff buff)
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
        public abstract object CalculateProperty(Entity entity, Buff buff, object value);
        public string PropertyName { get; set; }
        public object ConstValue { get; set; }
        public string UsingBuffPropertyName { get; set; }
        public ModifyOperator Operator { get; set; }
    }
}

namespace PVZEngine
{
    public abstract class Modifier
    {
        public virtual void PostAdd(Entity entity, Buff buff)
        {

        }
        public virtual void PostRemove(Entity entity, Buff buff)
        {

        }
        public abstract object CalculateProperty(Entity entity, Buff buff, object value);
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public ModifyOperator Operator { get; set; }
    }
}

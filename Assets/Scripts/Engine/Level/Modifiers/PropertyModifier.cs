using PVZEngine.Level;
using Tools;

namespace PVZEngine.Modifiers
{
    public interface IPropertyModifier
    {
        object GetModifierValue(Buff buff);
    }
    public abstract class PropertyModifier : IPropertyModifier
    {
        public PropertyModifier(string propertyName, object valueConst)
        {
            PropertyName = propertyName;
            ConstValue = valueConst;
        }
        public PropertyModifier(string propertyName, string buffPropertyName)
        {
            PropertyName = propertyName;
            UsingBuffPropertyName = buffPropertyName;
        }
        public virtual void PostAdd(Buff buff)
        {

        }
        public virtual void PostRemove(Buff buff)
        {

        }
        public object GetModifierValue(Buff buff)
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
        public string PropertyName { get; set; }
        public object ConstValue { get; set; }
        public string UsingBuffPropertyName { get; set; }
    }
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(string propertyName, T valueConst) : base(propertyName, valueConst)
        {
        }

        protected PropertyModifier(string propertyName, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
        public T GetModifierValueGeneric(Buff buff)
        {
            if (GetModifierValue(buff) is T tValue)
                return tValue;
            return default;
        }
    }
}

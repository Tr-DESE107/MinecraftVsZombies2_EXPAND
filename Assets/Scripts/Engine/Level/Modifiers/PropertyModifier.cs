using PVZEngine.Buffs;
using Tools;

namespace PVZEngine.Modifiers
{
    public interface IPropertyModifier
    {
        object GetModifierValue(IModifierContainer buff);
    }
    public abstract class PropertyModifier : IPropertyModifier
    {
        public PropertyModifier(PropertyKey propertyName, object valueConst)
        {
            PropertyName = propertyName;
            ConstValue = valueConst;
        }
        public PropertyModifier(PropertyKey propertyName, PropertyKey containerPropertyName)
        {
            PropertyName = propertyName;
            UsingContainerPropertyName = containerPropertyName;
        }
        public virtual void PostAdd(IModifierContainer container)
        {

        }
        public virtual void PostRemove(IModifierContainer container)
        {

        }
        public object GetModifierValue(IModifierContainer container)
        {
            if (PropertyKey.IsValid(UsingContainerPropertyName))
            {
                return container.GetProperty(UsingContainerPropertyName);
            }
            else
            {
                return ConstValue;
            }
        }
        public abstract ModifierCalculator GetCalculator();
        public PropertyKey PropertyName { get; set; }
        public object ConstValue { get; set; }
        public PropertyKey UsingContainerPropertyName { get; set; }
    }
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(PropertyKey propertyName, T valueConst) : base(propertyName, valueConst)
        {
        }

        protected PropertyModifier(PropertyKey propertyName, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
        }
        public T GetModifierValueGeneric(IModifierContainer container)
        {
            var value = GetModifierValue(container);
            if (value.TryToGeneric<T>(out var tValue))
                return tValue;
            return default;
        }
    }
}

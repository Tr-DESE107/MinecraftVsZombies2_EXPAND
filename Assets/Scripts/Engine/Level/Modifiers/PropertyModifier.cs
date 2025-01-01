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
        public PropertyModifier(string propertyName, object valueConst)
        {
            PropertyName = propertyName;
            ConstValue = valueConst;
        }
        public PropertyModifier(string propertyName, string containerPropertyName)
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
            if (!string.IsNullOrEmpty(UsingContainerPropertyName))
            {
                return container.GetProperty(UsingContainerPropertyName);
            }
            else
            {
                return ConstValue;
            }
        }
        public abstract ModifierCalculator GetCalculator();
        public string PropertyName { get; set; }
        public object ConstValue { get; set; }
        public string UsingContainerPropertyName { get; set; }
    }
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(string propertyName, T valueConst) : base(propertyName, valueConst)
        {
        }

        protected PropertyModifier(string propertyName, string buffPropertyName) : base(propertyName, buffPropertyName)
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

using PVZEngine.Buffs;

namespace PVZEngine.Modifiers
{
    public interface IPropertyModifier
    {
        object GetModifierValue(IModifierContainer buff);
    }
    public abstract class PropertyModifier : IPropertyModifier
    {
        public PropertyModifier(int priority)
        {
            Priority = priority;
        }
        public virtual void PostAdd(IModifierContainer container, IBuffTarget target)
        {

        }
        public virtual void PostRemove(IModifierContainer container, IBuffTarget target)
        {

        }
        public abstract object GetModifierValue(IModifierContainer container);
        public abstract ModifierCalculator GetCalculator();
        public abstract IPropertyKey PropertyName { get; }
        public abstract object ConstValue { get; }
        public abstract IPropertyKey UsingContainerPropertyName { get; }
        public int Priority { get; set; }
    }
    public abstract class PropertyModifier<T> : PropertyModifier
    {
        protected PropertyModifier(PropertyKey<T> propertyName, T valueConst, int priority) : base(priority)
        {
            PropertyNameGeneric = propertyName;
            ConstValueGeneric = valueConst;
        }

        protected PropertyModifier(PropertyKey<T> propertyName, PropertyKey<T> buffPropertyName, int priority) : base(priority)
        {
            PropertyNameGeneric = propertyName;
            UsingContainerPropertyNameGeneric = buffPropertyName;
        }
        public override object GetModifierValue(IModifierContainer container)
        {
            return GetModifierValueGeneric(container);
        }
        public T GetModifierValueGeneric(IModifierContainer container)
        {
            if (PropertyKeyHelper.IsValid(UsingContainerPropertyName))
            {
                return container.GetProperty<T>(UsingContainerPropertyNameGeneric);
            }
            else
            {
                return ConstValueGeneric;
            }
        }
        public PropertyKey<T> PropertyNameGeneric { get; }
        public T ConstValueGeneric { get; }
        public PropertyKey<T> UsingContainerPropertyNameGeneric { get; }
        public override IPropertyKey PropertyName => PropertyNameGeneric;
        public override object ConstValue => ConstValueGeneric;
        public override IPropertyKey UsingContainerPropertyName => UsingContainerPropertyNameGeneric;
    }
}

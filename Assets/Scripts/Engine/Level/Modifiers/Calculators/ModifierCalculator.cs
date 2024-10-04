using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using Tools;

namespace PVZEngine.Modifiers
{
    public abstract class ModifierCalculator
    {
        public abstract object Calculate(object value, IEnumerable<BuffModifierItem> modifiers);
        public T Calculate<T>(T value, IEnumerable<BuffModifierItem> modifiers)
        {
            var result = Calculate(value, modifiers);
            if (result.TryToGeneric<T>(out var tValue))
                return tValue;
            return value;
        }
    }
    public abstract class ModifierCalculator<TValue, TModifier> : ModifierCalculator where TModifier : PropertyModifier<TValue>
    {
        public override sealed object Calculate(object value, IEnumerable<BuffModifierItem> modifiers)
        {
            if (value is null)
                value = default(TValue);
            if (value is not TValue tValue)
                return value;
            return CalculateGeneric(tValue, modifiers.OfType<BuffModifierItem>());
        }
        public abstract TValue CalculateGeneric(TValue value, IEnumerable<BuffModifierItem> modifiers);
    }
    public struct BuffModifierItem
    {
        public BuffModifierItem(Buff buff, PropertyModifier modifier)
        {
            this.buff = buff;
            this.modifier = modifier;
        }
        public Buff buff;
        public PropertyModifier modifier;
    }
}

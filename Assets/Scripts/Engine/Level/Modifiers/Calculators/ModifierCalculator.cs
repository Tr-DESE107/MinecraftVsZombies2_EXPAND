using System.Collections.Generic;
using System.Linq;
using PVZEngine.Buffs;
using Tools;

namespace PVZEngine.Modifiers
{
    public abstract class ModifierCalculator
    {
        public abstract object Calculate(object value, IEnumerable<ModifierContainerItem> modifiers);
        public T Calculate<T>(T value, IEnumerable<ModifierContainerItem> modifiers)
        {
            var result = Calculate(value, modifiers);
            if (result.TryToGeneric<T>(out var tValue))
                return tValue;
            return value;
        }
    }
    public abstract class ModifierCalculator<TValue, TModifier> : ModifierCalculator where TModifier : PropertyModifier<TValue>
    {
        public override sealed object Calculate(object value, IEnumerable<ModifierContainerItem> modifiers)
        {
            if (!value.TryToGeneric<TValue>(out var tValue))
                return value;
            return CalculateGeneric(tValue, modifiers.OfType<ModifierContainerItem>());
        }
        public abstract TValue CalculateGeneric(TValue value, IEnumerable<ModifierContainerItem> modifiers);
    }
    public struct ModifierContainerItem
    {
        public ModifierContainerItem(IModifierContainer container, PropertyModifier modifier)
        {
            this.container = container;
            this.modifier = modifier;
        }
        public IModifierContainer container;
        public PropertyModifier modifier;
    }
}

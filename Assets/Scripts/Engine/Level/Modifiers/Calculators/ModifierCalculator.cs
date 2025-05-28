using System.Collections.Generic;
using System.Linq;
using Tools;

namespace PVZEngine.Modifiers
{
    public abstract class ModifierCalculator
    {
        public abstract object Calculate(object value, IEnumerable<ModifierContainerItem> modifiers);
    }
    public abstract class ModifierCalculator<TValue> : ModifierCalculator
    {
        public override sealed object Calculate(object value, IEnumerable<ModifierContainerItem> modifiers)
        {
            if (!value.TryToGeneric<TValue>(out var tValue))
                return value;
            return CalculateGeneric(tValue, modifiers.OfType<ModifierContainerItem>());
        }
        public abstract TValue CalculateGeneric(TValue value, IEnumerable<ModifierContainerItem> modifiers);
    }
    public abstract class ModifierCalculator<TValue, TModifier> : ModifierCalculator<TValue> where TModifier : PropertyModifier<TValue>
    {
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

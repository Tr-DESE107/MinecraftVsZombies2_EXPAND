using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;

namespace PVZEngine.Modifiers
{
    public abstract class SetModifierCalculator<T> : ModifierCalculator<T, PropertyModifier<T>>
    {
        public override T CalculateGeneric(T value, IEnumerable<BuffModifierItem> modifiers)
        {
            if (modifiers.Count() == 0)
                return value;

            var validModifiers = modifiers.Where(m => m.modifier is PropertyModifier<T>);
            if (validModifiers.Count() == 0)
                return value;
            var last = validModifiers.LastOrDefault();
            var buff = last.buff;
            var modifier = last.modifier as PropertyModifier<T>;
            return modifier.GetModifierValueGeneric(buff);
        }
    }
    public class BooleanCalculator : SetModifierCalculator<bool>
    {
    }
    public class NamespaceIDCalculator : SetModifierCalculator<NamespaceID>
    {
    }
    public class StringCalculator : SetModifierCalculator<string>
    {
    }
}

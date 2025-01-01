using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace PVZEngine.Modifiers
{
    public static class PropertyCalculator
    {
        public static object CalculateProperty(this IEnumerable<ModifierContainerItem> modifiers, string name, object value)
        {
            if (modifiers == null || modifiers.Count() == 0)
                return value;

            var calculators = modifiers.Select(p => p.modifier.GetCalculator()).Where(p => p != null).Distinct();
            ModifierCalculator calculator = null;
            foreach (var calc in calculators)
            {
                if (calculator != null)
                    throw new MultipleValueModifierException($"Modifiers of property {name} has multiple different calculators: {string.Join(',', calculators)}");
                calculator = calc;
            }
            if (calculator == null)
                throw new NullReferenceException($"Calculator for property {name} does not exists.");
            return calculator.Calculate(value, modifiers);
        }
    }
}

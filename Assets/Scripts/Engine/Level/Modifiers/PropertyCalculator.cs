﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Buffs;

namespace PVZEngine.Modifiers
{
    public static class PropertyCalculator
    {
        public static object CalculateProperty(this IEnumerable<ModifierContainerItem> modifiers, object value)
        {
            if (modifiers == null || modifiers.Count() == 0)
                return value;

            var calculators = modifiers.Select(p => p.modifier.GetCalculator()).Where(p => p != null).Distinct();
            ModifierCalculator calculator = null;
            foreach (var calc in calculators)
            {
                if (calculator != null)
                    throw new MultipleValueModifierException($"Modifiers of property has multiple different calculators: {string.Join(',', calculators)}");
                calculator = calc;
            }
            if (calculator == null)
                throw new NullReferenceException($"Calculator for property does not exists.");
            return calculator.Calculate(value, modifiers);
        }
        public static T CalculateProperty<T>(this IEnumerable<ModifierContainerItem> modifiers, T value)
        {
            if (modifiers == null || modifiers.Count() == 0)
                return value;

            var calculators = modifiers.Select(p => p.modifier.GetCalculator()).Where(p => p != null).Distinct();
            ModifierCalculator<T> calculator = null;
            foreach (var c in calculators)
            {
                if (c is not ModifierCalculator<T> calc)
                    continue;
                if (calculator != null)
                    throw new MultipleValueModifierException($"Modifiers of property has multiple different calculators: {string.Join(',', calculators)}");
                calculator = calc;
            }
            if (calculator == null)
                throw new NullReferenceException($"Calculator for property does not exists.");
            return calculator.CalculateGeneric(value, modifiers);
        }
    }
}

using System;
using System.Collections.Generic;

namespace PVZEngine.Modifiers
{
    public static class ModifierMap
    {
        public static void AddCalculator<T>(ModifierCalculator calculator) where T : PropertyModifier
        {
            AddCalculator(typeof(T), calculator);
        }
        public static void AddCalculator(Type type, ModifierCalculator calculator)
        {
            calculators.Add(type, calculator);
        }
        public static bool RemoveCalculator<T>() where T : PropertyModifier
        {
            return RemoveCalculator(typeof(T));
        }
        public static bool RemoveCalculator(Type type)
        {
            return calculators.Remove(type);
        }
        public static bool HasCalculator<T>() where T : PropertyModifier
        {
            return HasCalculator(typeof(T));
        }
        public static bool HasCalculator(Type type)
        {
            return calculators.ContainsKey(type);
        }
        public static ModifierCalculator GetCalculator<T>() where T : PropertyModifier
        {
            return GetCalculator(typeof(T));
        }
        public static ModifierCalculator GetCalculator(Type type)
        {
            if (calculators.TryGetValue(type, out var calc))
            {
                return calc;
            }
            return null;
        }
        private static readonly Dictionary<Type, ModifierCalculator> calculators = new Dictionary<Type, ModifierCalculator>();
    }
}

using PVZEngine.Entities;
using UnityEngine;

namespace PVZEngine.Armors
{
    [PropertyRegistryRegion(PropertyRegions.armor)]
    public static class EngineArmorProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta TINT = Get("tint");
        public static readonly PropertyMeta COLOR_OFFSET = Get("colorOffset");
        public static readonly PropertyMeta MAX_HEALTH = Get("maxHealth");
        public static readonly PropertyMeta SHELL = Get("shell");
        public static NamespaceID GetShellID(this Armor armor, bool ignoreBuffs = false)
        {
            return armor.GetProperty<NamespaceID>(SHELL, ignoreBuffs: ignoreBuffs);
        }
        public static Color GetTint(this Armor armor, bool ignoreBuffs = false)
        {
            return armor.GetProperty<Color>(TINT, ignoreBuffs: ignoreBuffs);
        }
        public static void SetTint(this Armor armor, Color value)
        {
            armor.SetProperty(TINT, value);
        }
        public static Color GetColorOffset(this Armor armor, bool ignoreBuffs = false)
        {
            return armor.GetProperty<Color>(COLOR_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public static void SetColorOffset(this Armor armor, Color value)
        {
            armor.SetProperty(COLOR_OFFSET, value);
        }
        public static float GetMaxHealth(this Armor armor, bool ignoreBuffs = false)
        {
            return armor.GetProperty<float>(MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
    }
}

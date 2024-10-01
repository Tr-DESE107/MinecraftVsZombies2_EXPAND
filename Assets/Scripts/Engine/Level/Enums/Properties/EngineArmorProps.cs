using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Definitions
{
    public static class EngineArmorProps
    {
        public const string TINT = "tint";
        public const string COLOR_OFFSET = "colorOffset";
        public const string MAX_HEALTH = "maxHealth";
        public const string SHELL = "shell";
        public static NamespaceID GetShellID(this Armor armor,bool ignoreBuffs = false)
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

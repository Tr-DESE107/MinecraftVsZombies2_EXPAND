namespace PVZEngine.Level
{
    public static class DamageFlags
    {
        public readonly static NamespaceID DAMAGE_BOTH_ARMOR_AND_BODY = Get("damageBothArmorAndBody");
        public readonly static NamespaceID DAMAGE_BODY_AFTER_ARMOR_BROKEN = Get("damageBodyAfterArmorBroken");
        public readonly static NamespaceID IGNORE_ARMOR = Get("ignoreArmor");
        public readonly static NamespaceID FALL_DAMAGE = Get("fallDamage");
        public static NamespaceID Get(string name)
        {
            return new NamespaceID("core", name);
        }
    }
}

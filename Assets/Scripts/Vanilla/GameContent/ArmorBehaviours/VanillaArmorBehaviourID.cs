using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Armors
{
    public static class VanillaArmorBehaviourNames
    {
        public const string damageState3 = "damage_state_3";
        public const string reflectiveBarrier = "reflective_barrier";
        public const string umbrellaShield = "umbrella_shield";
    }
    public static class VanillaArmorBehaviourID
    {
        public static readonly NamespaceID damageState3 = Get(VanillaArmorBehaviourNames.damageState3);
        public static readonly NamespaceID reflectiveBarrier = Get(VanillaArmorBehaviourNames.reflectiveBarrier);
        public static readonly NamespaceID umbrellaShield = Get(VanillaArmorBehaviourNames.umbrellaShield);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}

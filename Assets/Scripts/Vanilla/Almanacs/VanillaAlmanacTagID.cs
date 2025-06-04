using PVZEngine;

namespace MVZ2.Vanilla.Almanacs
{
    public class VanillaAlmanacTagID
    {
        public static readonly NamespaceID placementLand = Get("placement_land");
        public static readonly NamespaceID placementAquatic = Get("placement_aquatic");
        public static readonly NamespaceID placementSuspension = Get("placement_suspension");

        public static readonly NamespaceID lightSource = Get("light_source");
        public static readonly NamespaceID nocturnal = Get("nocturnal");
        public static readonly NamespaceID defensive = Get("defensive");
        public static readonly NamespaceID floorContraption = Get("floor_contraption");
        public static readonly NamespaceID low = Get("low");
        public static readonly NamespaceID high = Get("high");
        public static readonly NamespaceID flying = Get("flying");
        public static readonly NamespaceID canTrigger = Get("can_trigger");
        public static readonly NamespaceID fire = Get("fire");
        public static readonly NamespaceID loyal = Get("loyal");
        public static readonly NamespaceID paddle = Get("paddle");
        public static readonly NamespaceID controlImmunity = Get("control_immunity");

        public static readonly NamespaceID shell = Get("shell");
        public static readonly NamespaceID shieldShell = Get("shield_shell");
        public static readonly NamespaceID armorShell = Get("armor_shell");
        public static readonly NamespaceID mass = Get("mass");

        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}

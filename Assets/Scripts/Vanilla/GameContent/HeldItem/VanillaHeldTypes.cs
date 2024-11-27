using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.HeldItems
{
    public static class VanillaHeldItemNames
    {
        public const string entity = "entity";
        public const string pickaxe = "pickaxe";
        public const string starshard = "starshard";
        public const string trigger = "trigger";
    }
    public static class VanillaHeldTypes
    {
        public static readonly NamespaceID entity = Get(VanillaHeldItemNames.entity);
        public static readonly NamespaceID pickaxe = Get(VanillaHeldItemNames.pickaxe);
        public static readonly NamespaceID starshard = Get(VanillaHeldItemNames.starshard);
        public static readonly NamespaceID trigger = Get(VanillaHeldItemNames.trigger);
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}

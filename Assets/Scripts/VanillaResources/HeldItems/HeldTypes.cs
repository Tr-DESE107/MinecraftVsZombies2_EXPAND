using PVZEngine;

namespace MVZ2Logic.HeldItems
{
    public static class HeldItemNames
    {
        public const string none = "none";
        public const string blueprint = "blueprint";
        public const string entity = "entity";
        public const string pickaxe = "pickaxe";
        public const string starshard = "starshard";
    }
    public static class HeldTypes
    {
        public static readonly NamespaceID none = Get(HeldItemNames.none);
        public static readonly NamespaceID blueprint = Get(HeldItemNames.blueprint);
        public static readonly NamespaceID entity = Get(HeldItemNames.entity);
        public static readonly NamespaceID pickaxe = Get(HeldItemNames.pickaxe);
        public static readonly NamespaceID starshard = Get(HeldItemNames.starshard);
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}

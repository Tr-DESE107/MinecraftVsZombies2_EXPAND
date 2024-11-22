using PVZEngine;

namespace MVZ2.Vanilla.HeldItems
{
    public static class BuiltinHeldItemNames
    {
        public const string none = "none";
        public const string blueprint = "blueprint";
    }
    public static class BuiltinHeldTypes
    {
        public static readonly NamespaceID none = Get(BuiltinHeldItemNames.none);
        public static readonly NamespaceID blueprint = Get(BuiltinHeldItemNames.blueprint);
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}

using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class VanillaModelID
    {
        public static readonly NamespaceID moneyChest = Get("money_chest", ModelID.TYPE_ENTITY);
        public static readonly NamespaceID blueprintPickup = Get("blueprint_pickup", ModelID.TYPE_ENTITY);
        public static readonly NamespaceID mapPickup = Get("map_pickup", ModelID.TYPE_ENTITY);
        public static readonly NamespaceID emerald = Get("emerald", ModelID.TYPE_ENTITY);
        public static readonly NamespaceID ruby = Get("ruby", ModelID.TYPE_ENTITY);
        public static readonly NamespaceID diamond = Get("diamond", ModelID.TYPE_ENTITY);
        private static NamespaceID Get(string name, string type)
        {
            return new NamespaceID(VanillaMod.spaceName, name).ToModelID(type);
        }
    }
}

using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaModelID
    {
        public static readonly NamespaceID moneyChest = Get("money_chest", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID blueprintPickup = Get("blueprint_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID mapPickup = Get("map_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID emerald = Get("emerald", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID ruby = Get("ruby", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID diamond = Get("diamond", EngineModelID.TYPE_ENTITY);
        private static NamespaceID Get(string name, string type)
        {
            return new NamespaceID(VanillaMod.spaceName, name).ToModelID(type);
        }
    }
}

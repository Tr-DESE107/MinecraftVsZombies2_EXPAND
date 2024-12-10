using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Models
{
    public static class VanillaModelID
    {
        public const string TYPE_HELD_ITEM = "held";

        public static readonly NamespaceID moneyChest = Get("money_chest", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID blueprintPickup = Get("blueprint_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID mapPickup = Get("map_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID emerald = Get("emerald", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID ruby = Get("ruby", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID diamond = Get("diamond", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID pistonPalm = Get("piston_palm", EngineModelID.TYPE_ENTITY);

        
        public static readonly NamespaceID pickaxeHeldItem = Get("pickaxe", TYPE_HELD_ITEM);
        public static readonly NamespaceID triggerHeldItem = Get("trigger", TYPE_HELD_ITEM);
        public static readonly NamespaceID swordHeldItem = Get("sword", TYPE_HELD_ITEM);
        public static readonly NamespaceID defaultStartShardHeldItem = Get("starshard.default", TYPE_HELD_ITEM);
        public static NamespaceID GetStarshardHeldItem(NamespaceID areaID)
        {
            return new NamespaceID(areaID.spacename, $"starshard.{areaID.path}").ToModelID(TYPE_HELD_ITEM);
        }
        private static NamespaceID Get(string name, string type)
        {
            return new NamespaceID(VanillaMod.spaceName, name).ToModelID(type);
        }
    }
}

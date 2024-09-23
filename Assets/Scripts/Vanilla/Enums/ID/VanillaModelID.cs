using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class VanillaModelID
    {
        public static readonly NamespaceID moneyChest = Get("money_chest", "entity");
        public static readonly NamespaceID blueprintPickup = Get("blueprint_pickup", "entity");
        private static NamespaceID Get(string name, string type)
        {
            return new NamespaceID(VanillaMod.spaceName, name).ToModelID(type);
        }
    }
}

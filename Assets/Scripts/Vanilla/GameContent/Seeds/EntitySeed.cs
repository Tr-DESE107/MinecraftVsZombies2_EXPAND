using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(string nsp, string name, int cost, NamespaceID rechargeID, bool triggerActive = false, bool canInstantTrigger = false) : base(nsp, name)
        {
            SetProperty(VanillaSeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(VanillaSeedProps.SEED_ENTITY_ID, new NamespaceID(nsp, name));
            SetProperty(EngineSeedProps.COST, cost);
            SetProperty(EngineSeedProps.RECHARGE_ID, rechargeID);
            SetProperty(VanillaSeedProps.TRIGGER_ACTIVE, triggerActive);
            SetProperty(VanillaSeedProps.CAN_INSTANT_TRIGGER, canInstantTrigger);
        }
    }
}

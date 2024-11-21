using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(string nsp, string name, int cost, NamespaceID rechargeID, bool triggerActive = false, int triggerCost = 0) : base(nsp, name)
        {
            SetProperty(BuiltinSeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(BuiltinSeedProps.SEED_ENTITY_ID, new NamespaceID(nsp, name));
            SetProperty(EngineSeedProps.COST, cost);
            SetProperty(EngineSeedProps.RECHARGE_ID, rechargeID);
            SetProperty(BuiltinSeedProps.TRIGGER_ACTIVE, triggerActive);
            SetProperty(BuiltinSeedProps.TRIGGER_COST, triggerCost);
        }
    }
}

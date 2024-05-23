using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(NamespaceID entityID, int cost, NamespaceID rechargeID, bool triggerActive = false, int triggerCost = 0) : base()
        {
            SetProperty(SeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(SeedProps.SEED_ENTITY_ID, entityID);
            SetProperty(SeedProperties.COST, cost);
            SetProperty(SeedProperties.RECHARGE_ID, rechargeID);
            SetProperty(SeedProps.TRIGGER_ACTIVE, triggerActive);
            SetProperty(SeedProps.TRIGGER_COST, triggerCost);
        }
    }
}

using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(NamespaceID entityID, int cost, int startRecharge = 0, int maxRecharge = 0, bool triggerActive = false, int triggerCost = 0) : base()
        {
            SetProperty(SeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(SeedProps.SEED_ENTITY_ID, entityID);
            SetProperty(SeedProperties.COST, cost);
            SetProperty(SeedProperties.START_RECHARGE_TIME, startRecharge);
            SetProperty(SeedProperties.RECHARGE_TIME, maxRecharge);
            SetProperty(SeedProps.TRIGGER_ACTIVE, triggerActive);
            SetProperty(SeedProps.TRIGGER_COST, triggerCost);
        }
    }
}

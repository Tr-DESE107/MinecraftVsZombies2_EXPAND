using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(string nsp, string name, int cost, NamespaceID rechargeID, bool triggerActive = false, int triggerCost = 0) : base(nsp, name)
        {
            SetProperty(SeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(SeedProps.SEED_ENTITY_ID, new NamespaceID(nsp, name));
            SetProperty(SeedProperties.COST, cost);
            SetProperty(SeedProperties.RECHARGE_ID, rechargeID);
            SetProperty(SeedProps.TRIGGER_ACTIVE, triggerActive);
            SetProperty(SeedProps.TRIGGER_COST, triggerCost);
        }
    }
}

using System;
using PVZEngine;

namespace MVZ2.Vanilla.SeedPacks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntitySeedDefinitionAttribute : Attribute
    {
        public EntitySeedDefinitionAttribute(int cost, string rechargeNamespace, string rechargeName, bool triggerActive = false, int triggerCost = 0)
        {
            Cost = cost;
            RechargeID = new NamespaceID(rechargeNamespace, rechargeName);
            TriggerActive = triggerActive;
            TriggerCost = triggerCost;
        }
        public int Cost { get; }
        public NamespaceID RechargeID { get; }
        public bool TriggerActive { get; }
        public int TriggerCost { get; }
    }
}

using System;
using PVZEngine;

namespace MVZ2.Vanilla.SeedPacks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntitySeedDefinitionAttribute : Attribute
    {
        public EntitySeedDefinitionAttribute(int cost, string rechargeNamespace, string rechargeName)
        {
            Cost = cost;
            RechargeID = new NamespaceID(rechargeNamespace, rechargeName);
        }
        public int Cost { get; }
        public NamespaceID RechargeID { get; }
    }
}

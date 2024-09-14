using PVZEngine.Base;

namespace PVZEngine.Definitions
{
    public class SeedDefinition : Definition
    {
        public SeedDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public int GetCost()
        {
            return GetProperty<int>(SeedProperties.COST);
        }
        public NamespaceID GetRechargeID()
        {
            return GetProperty<NamespaceID>(SeedProperties.RECHARGE_ID);
        }
    }
}

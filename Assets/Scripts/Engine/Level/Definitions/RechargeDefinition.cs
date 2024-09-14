using PVZEngine.Base;

namespace PVZEngine.Definitions
{
    public class RechargeDefinition : Definition
    {
        public RechargeDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public int GetStartMaxRecharge()
        {
            return GetProperty<int>(RechargeProperties.START_MAX_RECHARGE);
        }
        public int GetMaxRecharge()
        {
            return GetProperty<int>(RechargeProperties.MAX_RECHARGE);
        }
    }
}

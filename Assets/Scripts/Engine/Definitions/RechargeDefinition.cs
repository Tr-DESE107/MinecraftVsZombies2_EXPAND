namespace PVZEngine
{
    public class RechargeDefinition : Definition
    {
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

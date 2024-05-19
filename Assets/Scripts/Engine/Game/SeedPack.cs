using UnityEngine;

namespace PVZEngine
{
    public class SeedPack
    {
        public SeedPack(NamespaceID seedRef)
        {
            SeedReference = seedRef;
        }
        public void FullRecharge()
        {
            Recharge = MaxRecharge;
        }
        public void ResetRecharge()
        {
            Recharge = 0;
        }
        public void UpdateRecharge(float rechargeSpeed)
        {
            if (Recharge >= MaxRecharge)
                return;
            Recharge += rechargeSpeed;
            Recharge = Mathf.Min(MaxRecharge, Recharge);
        }
        public NamespaceID SeedReference { get; set; }
        public int Cost { get; set; }
        public float MaxRecharge { get; set; }
        public float Recharge { get; set; }
    }
}

using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine.LevelManaging
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
        public bool IsCharged()
        {
            return Recharge >= MaxRecharge;
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
        public SerializableSeedPack Serialize()
        {
            return new SerializableSeedPack()
            {
                seedID = SeedReference,
                cost = Cost,
                recharge = Recharge,
                maxRecharge = MaxRecharge,
            };
        }
        public static SeedPack Deserialize(SerializableSeedPack seri)
        {
            return new SeedPack(seri.seedID)
            {
                Cost = seri.cost,
                Recharge = seri.recharge,
                MaxRecharge = seri.maxRecharge
            };
        }
        public NamespaceID SeedReference { get; set; }
        public int Cost { get; set; }
        public float MaxRecharge { get; set; }
        public float Recharge { get; set; }
    }
}

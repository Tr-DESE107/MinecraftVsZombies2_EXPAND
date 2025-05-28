using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.SeedPacks
{
    public class ClassicSeedPack : SeedPack
    {
        public ClassicSeedPack(LevelEngine level, SeedDefinition definition, long id) : base(level, definition, id)
        {
        }
        public int GetIndex()
        {
            return Level.GetSeedPackIndex(this);
        }
        public override BuffReference GetBuffReference(Buff buff)
        {
            return new BuffReferenceClassicSeedPack(ID, buff.ID);
        }
        protected override void OnUpdate(float rechargeSpeed)
        {
            base.OnUpdate(rechargeSpeed);
            if (!this.IsCharged())
            {
                var recharge = this.GetRecharge();
                recharge += rechargeSpeed * this.GetRechargeSpeed();
                recharge = Mathf.Min(this.GetMaxRecharge(), recharge);
                this.SetRecharge(recharge);
            }
        }
        #region 序列化
        public SerializableClassicSeedPack Serialize()
        {
            var seri = new SerializableClassicSeedPack();
            ApplySerializableProperties(seri);
            return seri;
        }
        public static ClassicSeedPack Deserialize(SerializableClassicSeedPack seri, LevelEngine level)
        {
            var definition = level.Content.GetSeedDefinition(seri.seedID);
            var seedPack = new ClassicSeedPack(level, definition, seri.id);
            seedPack.ApplyDeserializedProperties(level, seri);
            return seedPack;
        }
        #endregion
    }
}

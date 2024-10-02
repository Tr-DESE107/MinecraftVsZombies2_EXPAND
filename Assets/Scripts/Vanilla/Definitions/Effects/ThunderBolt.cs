using MVZ2.Extensions;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.thunderBolt)]
    public class ThunderBolt : VanillaEffect
    {

        #region 公有方法
        public ThunderBolt(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 30;
            entity.Level.PlaySound(SoundID.thunder);
            entity.AddBuff<ThunderLightFadeoutBuff>();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var tint = entity.GetTint();
            tint.a = entity.Timeout < 0 ? 1 : entity.Timeout / 30f;
            entity.SetTint(tint);
        }
        #endregion
    }
}
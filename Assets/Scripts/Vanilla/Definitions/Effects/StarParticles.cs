using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [Definition(EffectNames.starParticles)]
    public class StarParticles : VanillaEffect
    {

        #region 公有方法
        public StarParticles(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            entity.Timeout = 30;
        }
        #endregion
    }
}
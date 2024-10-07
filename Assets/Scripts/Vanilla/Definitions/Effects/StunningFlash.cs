using MVZ2.Vanilla;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.stunningFlash)]
    public class StunningFlash : VanillaEffect
    {
        #region 公有方法
        public StunningFlash(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 15;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.RenderScale = Vector3.one * entity.Timeout;
        }
        #endregion
    }
}
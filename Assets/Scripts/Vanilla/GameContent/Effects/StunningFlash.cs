using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.stunningFlash)]
    public class StunningFlash : EffectBehaviour
    {
        #region 公有方法
        public StunningFlash(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.RenderScale = Vector3.one * entity.Timeout;
        }
        #endregion
    }
}
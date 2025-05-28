using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.castleTwilight)]
    public class CastleTwilight : EffectBehaviour
    {
        #region 公有方法
        public CastleTwilight(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.Timeout < 0)
            {
                entity.SetTint(Color.white);
            }
            else
            {
                entity.SetTint(new Color(1, 1, 1, entity.Timeout / 30f));
            }
        }
        #endregion
    }
}
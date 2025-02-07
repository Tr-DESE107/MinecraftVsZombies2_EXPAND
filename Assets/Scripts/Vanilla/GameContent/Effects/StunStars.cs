using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.stunStars)]
    public class StunStars : EffectBehaviour
    {
        #region 公有方法
        public StunStars(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (parent == null || !parent.Exists() || parent.IsDead)
            {
                entity.Remove();
                return;
            }
            var pos = parent.Position;
            entity.Position = pos + Vector3.up * parent.GetScaledSize().y;
        }
        #endregion
    }
}
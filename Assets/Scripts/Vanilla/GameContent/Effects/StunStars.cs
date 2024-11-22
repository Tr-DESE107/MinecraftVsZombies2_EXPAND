using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.stunStars)]
    public class StunStars : VanillaEffect
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
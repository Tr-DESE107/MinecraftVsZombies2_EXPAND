using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.stunStars)]
    public class StunStars : EffectBehaviour
    {
        #region ���з���
        public StunStars(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (!parent.ExistsAndAlive())
            {
                entity.Remove();
                return;
            }
            entity.Position = GetPosition(parent);
        }
        public static Vector3 GetPosition(Entity parent)
        {
            return parent.Position + Vector3.up * parent.GetScaledSize().y;
        }
        #endregion
    }
}
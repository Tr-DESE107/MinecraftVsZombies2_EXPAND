using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.shineRing)]
    public class ShineRing : EffectBehaviour
    {
        #region ���з���
        public ShineRing(string nsp, string name) : base(nsp, name)
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
            if (!parent.IsLightSource())
            {
                entity.Remove();
                return;
            }
            entity.Position = parent.GetCenter();
            var color = parent.GetLightColor();
            color.a *= 0.3f;
            entity.SetTint(color);
            var lightRange = parent.GetLightRange() / 100f;
            lightRange.y = lightRange.z;
            entity.SetDisplayScale(lightRange);
        }
        #endregion
    }
}
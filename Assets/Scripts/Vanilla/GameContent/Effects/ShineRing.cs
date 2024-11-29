using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.shineRing)]
    public class ShineRing : EffectBehaviour
    {
        #region 公有方法
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
            entity.Position = parent.GetBoundsCenter();
            var color = parent.GetLightColor();
            color.a *= 0.3f;
            entity.SetTint(color);
            var lightRange = parent.GetLightRange() / 100f;
            lightRange.y = lightRange.z;
            entity.RenderScale = lightRange;
        }
        #endregion
    }
}
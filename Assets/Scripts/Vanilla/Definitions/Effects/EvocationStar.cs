using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.evocationStar)]
    public class EvocationStar : VanillaEffect
    {
        #region 公有方法
        public EvocationStar(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 45;
            entity.Level.PlaySound(SoundID.evocation);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var scale = entity.RNG.Next(0.98f, 1.12f);
            entity.RenderScale = Vector3.one * scale;
        }
        #endregion
    }
}
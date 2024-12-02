using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.evocationStar)]
    public class EvocationStar : EffectBehaviour
    {
        #region 公有方法
        public EvocationStar(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.PlaySound(VanillaSoundID.evocation);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var scale = entity.RNG.Next(0.98f, 1.12f);
            entity.SetDisplayScale(Vector3.one * scale);
        }
        #endregion
    }
}
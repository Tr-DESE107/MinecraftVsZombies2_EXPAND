using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.spiritUniverse)]
    public class SpiritUniverse : EffectBehaviour
    {
        #region 公有方法
        public SpiritUniverse(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var tint = entity.GetTint();
            tint.a = 0;
            entity.SetTint(tint);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var tint = entity.GetTint();
            var speed = 1 / 90f;
            if ((entity.Timeout > 0 && entity.Timeout <= 30) || (entity.Level.IsGameOver() || !entity.Level.IsGameStarted()))
            {
                speed = -1 / 30f;
            }
            tint.a = Mathf.Clamp01(tint.a + speed);
            entity.SetTint(tint);
        }
    }
}
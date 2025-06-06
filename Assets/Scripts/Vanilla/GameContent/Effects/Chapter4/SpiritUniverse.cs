using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
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
            AddAura(new NightAura());
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
        public class NightAura : AuraEffectDefinition
        {
            public NightAura()
            {
                BuffID = VanillaBuffID.Level.spiritUniverseNight;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                results.Add(auraEffect.Level);
            }

            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                var entity = effect?.Source?.GetEntity();
                if (!entity.ExistsAndAlive())
                    return;
                var alpha = entity.GetTint().a;
                var color = Color.Lerp(Color.white, Color.black, alpha * 0.98f);
                SpiritUniverseNightBuff.SetBackgroundLightMultiplier(buff, color);
            }
        }
    }
}
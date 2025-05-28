using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.thunderCloud)]
    public class ThunderCloud : EffectBehaviour
    {
        #region 公有方法
        public ThunderCloud(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);

            var size = entity.GetScaledSize();
            if (entity.Timeout > DISAPPEAR_TIMEOUT && entity.Timeout <= THUNDER_TIMEOUT)
            {
                var angle = entity.RNG.Next(360f);
                var radius = entity.RNG.Next(Mathf.Min(size.x, size.z) * 0.5f);
                var pos2D = Vector2.right.RotateClockwise(angle) * radius;
                var pos = entity.GetCenter() + new Vector3(pos2D.x, 0, pos2D.y);
                var targetPos = pos;
                targetPos.y = entity.Level.GetGroundY(pos.x, pos.z);
                TeslaCoil.Shock(entity, entity.GetDamage(), entity.GetFaction(), SHOCK_RADIUS, targetPos, new DamageEffectList(VanillaDamageEffects.LIGHTNING, VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY, VanillaDamageEffects.MUTE));
                TeslaCoil.CreateArc(entity, pos, targetPos);

                var param = entity.GetSpawnParams();
                param.SetProperty(EngineEntityProps.SIZE, Vector3.one * SHOCK_RADIUS * 2);
                var explosion = entity.Spawn(VanillaEffectID.explosion, targetPos, param);

                if (entity.Timeout % 3 == 0)
                {
                    entity.PlaySound(VanillaSoundID.thunder, entity.RNG.Next(0.75f, 1.25f));
                    entity.PlaySound(VanillaSoundID.smash, entity.RNG.Next(0.75f, 1.25f), 0.5f);
                    entity.Level.ShakeScreen(5, 0, 3);
                }
            }

            entity.SetModelProperty("Size", size);
            entity.SetModelProperty("Stopped", entity.Timeout <= DISAPPEAR_TIMEOUT);
        }
        #endregion

        public const float SHOCK_RADIUS = 40;
        public const int THUNDER_TIMEOUT = DISAPPEAR_TIMEOUT + 90;
        public const int DISAPPEAR_TIMEOUT = 30;
    }
}
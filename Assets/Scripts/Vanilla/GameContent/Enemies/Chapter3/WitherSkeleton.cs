using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.WitherSkeleton)]
    public class WitherSkeleton : MeleeEnemy
    {
        public WitherSkeleton(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var level = entity.Level;
            if (level.IsWaterLane(entity.GetLane()))
            {
                entity.AddBuff<BoatBuff>();
                entity.SetAnimationBool("HasBoat", true);
            }

        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
            entity.SetAnimationBool("HasBoat", entity.HasBuff<BoatBuff>());
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // �����鴬������
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity);
                effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                effect.ChangeModel(VanillaModelID.boatItem);
                effect.SetDisplayScale(entity.GetDisplayScale());
            }
            else
            {
                if (info.Effects.HasEffect(VanillaDamageEffects.DROWN))
                {
                    Global.Game.Unlock(VanillaUnlockID.rickrollDrown);
                }
            }
        }
    }
}

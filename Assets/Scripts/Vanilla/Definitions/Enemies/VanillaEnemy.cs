using MVZ2.Extensions;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    public abstract class VanillaEnemy : VanillaEntity
    {
        public VanillaEnemy(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.Level.CreateBuff<RandomEnemySpeedBuff>();
            buff.SetProperty(RandomEnemySpeedBuff.PROP_SPEED, GetRandomSpeedMultiplier(entity));
            entity.AddBuff(buff);

            entity.SetFaction(entity.Level.Option.RightFaction);

            entity.CollisionMask = EntityCollision.MASK_PLANT
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS;
        }
        public override void Update(Entity enemy)
        {
            base.Update(enemy);
            Vector3 pos = enemy.Position;
            pos.x = Mathf.Min(pos.x, BuiltinLevel.GetEnemyRightBorderX());
            enemy.Position = pos;

            enemy.SetAnimationFloat("AttackSpeed", enemy.GetAttackSpeed());
            enemy.SetAnimationFloat("MoveSpeed", enemy.GetSpeed());
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.PlaySound(entity.GetDeathSound());
        }
        protected virtual float GetRandomSpeedMultiplier(Entity entity)
        {
            return entity.RNG.Next(1, 1.5f);
        }
        public override int Type => EntityTypes.ENEMY;
    }
}
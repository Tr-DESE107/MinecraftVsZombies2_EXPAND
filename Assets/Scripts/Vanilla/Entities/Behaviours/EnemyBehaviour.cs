using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class EnemyBehaviour : VanillaEntityBehaviour
    {
        public EnemyBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.CreateBuff<RandomEnemySpeedBuff>();
            buff.SetProperty(RandomEnemySpeedBuff.PROP_SPEED, GetRandomSpeedMultiplier(entity));
            entity.AddBuff(buff);

            entity.SetFaction(entity.Level.Option.RightFaction);

            entity.CollisionMask = EntityCollision.MASK_PLANT
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS;
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen())
            {
                UpdateAI(entity);
            }
            UpdateLogic(entity);
        }
        protected virtual void UpdateLogic(Entity entity)
        {
            Vector3 pos = entity.Position;
            pos.x = Mathf.Min(pos.x, VanillaLevelExt.GetEnemyRightBorderX());
            entity.Position = pos;
            var attackSpeed = entity.GetAttackSpeed();
            var speed = entity.GetSpeed();
            if (entity.IsAIFrozen())
            {
                attackSpeed = 0;
                speed = 0;
            }
            entity.SetAnimationFloat("AttackSpeed", attackSpeed);
            entity.SetAnimationFloat("MoveSpeed", speed);
        }
        protected virtual void UpdateAI(Entity entity)
        {
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DamageInput damageInfo)
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
    }
}
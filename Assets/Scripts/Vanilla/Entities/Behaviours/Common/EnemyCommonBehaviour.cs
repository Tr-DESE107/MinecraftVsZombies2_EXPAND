using System;
using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.enemyCommon)]
    public class EnemyCommonBehaviour : EntityBehaviourDefinition
    {
        public EnemyCommonBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.CreateBuff<RandomEnemySpeedBuff>();
            RandomEnemySpeedBuff.SetSpeed(buff, entity.RNG.Next(1, 1.5f));
            entity.AddBuff(buff);

            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT
                | EntityCollisionHelper.MASK_ENEMY
                | EntityCollisionHelper.MASK_OBSTACLE
                | EntityCollisionHelper.MASK_BOSS;

            var startingArmor = entity.GetStartingArmor();
            var startingShield = entity.GetStartingShield();
            if (NamespaceID.IsValid(startingArmor))
            {
                entity.EquipMainArmor(startingArmor);
            }
            if (NamespaceID.IsValid(startingShield))
            {
                entity.EquipArmorTo(VanillaArmorSlots.shield, startingShield);
            }
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            bool remove = false;
            if (!entity.IsFacingLeft() && IsOutsideRight(entity))
            {
                remove = true;
            }
            else if (entity.Level.IsIZombie() && IsOutsideLeft(entity))
            {
                remove = true;
            }
            if (remove)
            {
                entity.Neutralize();
                entity.Remove();
                return;
            }

            if (entity.IsFacingLeft())
            {
                Vector3 pos = entity.Position;
                if (pos.x > VanillaLevelExt.GetEnemyRightBorderX())
                {
                    pos.x = Mathf.Min(pos.x, VanillaLevelExt.GetEnemyRightBorderX());
                    var vel = entity.Velocity;
                    vel.x = Mathf.Min(vel.x, 0);
                    entity.Velocity = vel;
                }
                entity.Position = pos;
            }

            if (entity.IsOnGround)
            {
                entity.CheckAlignWithLane();
            }

            var scale = entity.GetFinalDisplayScale();
            var scaleX = Mathf.Abs(scale.x);
            var attackSpeed = entity.GetAttackSpeed() / entity.GetAttackSpeed(ignoreBuffs: true) / scaleX;
            var speed = entity.GetSpeed() / entity.GetSpeed(ignoreBuffs: true) / scaleX;
            if (entity.IsAIFrozen())
            {
                attackSpeed = 0;
                speed = 0;
            }
            entity.SetAnimationFloat("AttackSpeed", attackSpeed);
            entity.SetAnimationFloat("MoveSpeed", speed);
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null && bodyResult.Amount > 0 && !bodyResult.HasEffect(VanillaDamageEffects.NO_DAMAGE_BLINK))
            {
                var entity = bodyResult.Entity;
                entity.DamageBlink();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.DamageBlink();
            entity.PlayCrySound(entity.GetDeathSound());
        }
        protected virtual bool IsOutsideLeft(Entity enemy)
        {
            var bounds = enemy.GetBounds();
            var position = enemy.Position;
            return bounds.max.x < VanillaLevelExt.ENEMY_LEFT_BORDER;
        }
        protected virtual bool IsOutsideRight(Entity enemy)
        {
            var bounds = enemy.GetBounds();
            var position = enemy.Position;
            return bounds.min.x > VanillaLevelExt.ENEMY_RIGHT_BORDER;
        }
    }
}
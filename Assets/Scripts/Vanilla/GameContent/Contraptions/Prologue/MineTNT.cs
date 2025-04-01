using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.mineTNT)]
    public class MineTNT : ContraptionBehaviour
    {
        public MineTNT(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            var riseTimer = new FrameTimer(450);
            SetRiseTimer(entity, riseTimer);
            entity.SetAnimationBool("Ready", riseTimer.Frame < 30);

            entity.CollisionMaskHostile |= EntityCollisionHelper.MASK_ENEMY;
        }

        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            RiseUpdate(entity);
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var riseTimer = GetRiseTimer(entity);
            if (riseTimer.Frame > 30)
            {
                riseTimer.Frame = 30;
            }
            List<LawnGrid> grids = new List<LawnGrid>();
            for (int x = 0; x < entity.Level.GetMaxColumnCount(); x++)
            {
                for (int y = 0; y < entity.Level.GetMaxLaneCount(); y++)
                {
                    var grid = entity.Level.GetGrid(x, y);
                    if (grid.CanPlaceOrStackEntity(VanillaContraptionID.mineTNT))
                    {
                        grids.Add(grid);
                    }
                }
            }
            var groups = grids.GroupBy(g => g.Column).OrderByDescending(g => g.Key).Take(2);
            var selectedGrids = groups.SelectMany(g => g.Shuffle(entity.RNG)).Take(2);
            foreach (var grid in selectedGrids)
            {
                FireSeed(entity, grid);
            }
        }

        public static FrameTimer GetRiseTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_RISE_TIMER);
        }

        public static void SetRiseTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_RISE_TIMER, timer);
        }

        private void RiseUpdate(Entity entity)
        {
            var riseTimer = GetRiseTimer(entity);
            riseTimer.Run(entity.GetAttackSpeed());

            if (riseTimer.Frame == 30)
            {
                entity.PlaySound(VanillaSoundID.dirtRise);
            }
            if (riseTimer.Frame < 30 && !riseTimer.Expired)
            {
                if (!entity.HasBuff<MineTNTInvincibleBuff>())
                    entity.AddBuff<MineTNTInvincibleBuff>();
            }
            else
            {
                if (entity.HasBuff<MineTNTInvincibleBuff>())
                    entity.RemoveBuffs<MineTNTInvincibleBuff>();
            }
            entity.SetAnimationBool("Ready", riseTimer.Frame < 30);
        }

        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            var other = collision.Other;
            if (!other.IsVulnerableEntity() || !other.ExistsAndAlive())
                return;
            var self = collision.Entity;
            if (!self.IsHostile(other))
                return;
            var otherCollider = collision.OtherCollider;
            if (!otherCollider.IsMain())
                return;
            var riseTimer = GetRiseTimer(self);
            if (riseTimer == null || !riseTimer.Expired)
                return;

            
            var damageEffects = new DamageEffectList(
                VanillaDamageEffects.MUTE,
                VanillaDamageEffects.IGNORE_ARMOR,
                // VanillaDamageEffects.REMOVE_ON_DEATH,
                VanillaDamageEffects.EXPLOSION
            );

            // 获取爆炸范围并执行爆炸
            float range = self.GetRange();
            var damageOutputs = self.Level.Explode(
                self.Position,
                range,
                self.GetFaction(),
                self.GetDamage(),
                damageEffects,
                self
            );

            // 移植TNT的击飞逻辑
            foreach (var output in damageOutputs)
            {
                var result = output?.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - self.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }

            // 同步TNT特效
            var explosion = self.Level.Spawn(VanillaEffectID.explosion, self.GetCenter(), self);
            explosion.SetSize(Vector3.one * (range * 2));
            self.PlaySound(VanillaSoundID.explosion); // 使用TNT爆炸音效
            
            self.Level.Spawn(VanillaEffectID.mineDebris, self.Position, self);
            self.Remove();
            self.Level.ShakeScreen(10, 0, 15);
        }

        private static Entity FireSeed(Entity contraption, LawnGrid grid)
        {
            var level = contraption.Level;
            var seed = level.Spawn(VanillaProjectileID.mineTNTSeed, contraption.Position, contraption);

            var x = level.GetEntityColumnX(grid.Column);
            var z = level.GetEntityLaneZ(grid.Lane);
            var y = level.GetGroundY(x, z);
            var target = new Vector3(x, y, z);
            var maxY = Mathf.Max(contraption.Position.y, y) + 32;
            seed.Velocity = VanillaProjectileExt.GetLobVelocity(contraption.Position, target, maxY, seed.GetGravity());

            return seed;
        }

        private static readonly NamespaceID ID = VanillaContraptionID.mineTNT;
        private static readonly VanillaEntityPropertyMeta PROP_RISE_TIMER = new VanillaEntityPropertyMeta("RiseTimer");
    }
}
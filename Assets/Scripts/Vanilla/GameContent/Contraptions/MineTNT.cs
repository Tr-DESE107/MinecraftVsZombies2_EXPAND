using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.mineTNT)]
    [EntitySeedDefinition(25, VanillaMod.spaceName, VanillaRechargeNames.longTime)]
    public class MineTNT : VanillaContraption
    {
        public MineTNT(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var riseTimer = new FrameTimer(450);
            SetRiseTimer(entity, riseTimer);

            entity.CollisionMask |= EntityCollision.MASK_ENEMY;
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
                    if (grid.CanPlace(VanillaContraptionID.mineTNT))
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
            return entity.GetProperty<FrameTimer>("RiseTimer");
        }
        public static void SetRiseTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("RiseTimer", timer);
        }
        private void RiseUpdate(Entity entity)
        {
            var riseTimer = GetRiseTimer(entity);
            riseTimer.Run(entity.GetAttackSpeed());

            if (riseTimer.Frame == 30)
            {
                entity.PlaySound(VanillaSoundID.dirtRise);
            }
            if (riseTimer.Frame < 30)
            {
                entity.SetAnimationBool("Ready", true);
            }
            if (riseTimer.Frame < 30 && !riseTimer.Expired)
            {
                if (!entity.HasBuff<MineTNTInvincibleBuff>())
                    entity.AddBuff<MineTNTInvincibleBuff>();
            }
            else
            {
                if (entity.HasBuff<MineTNTInvincibleBuff>())
                    entity.RemoveBuffs(entity.GetBuffs<MineTNTInvincibleBuff>());
            }
        }
        public override void PostCollision(Entity entity, Entity other, int state)
        {
            base.PostCollision(entity, other, state);
            if (state == EntityCollision.STATE_EXIT)
                return;
            if (!EntityTypes.IsDamagable(entity.Type))
                return;
            if (!entity.IsEnemy(other))
                return;
            var riseTimer = GetRiseTimer(entity);
            if (riseTimer == null || !riseTimer.Expired)
                return;
            entity.Level.Explode(entity.Position, EXPLOSION_RADIUS, entity.GetFaction(), 1800, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.REMOVE_ON_DEATH), new EntityReferenceChain(entity));
            entity.Level.Spawn<MineDebris>(entity.Position, entity);
            entity.Remove();
            entity.PlaySound(VanillaSoundID.mineExplode);
            entity.Level.ShakeScreen(10, 0, 15);
        }
        private static Entity FireSeed(Entity contraption, LawnGrid grid)
        {
            var level = contraption.Level;

            var seed = level.Spawn<MineTNTSeed>(contraption.Position, contraption);

            var x = level.GetEntityColumnX(grid.Column);
            var z = level.GetEntityLaneZ(grid.Lane);
            var y = level.GetGroundY(x, z);
            var target = new Vector3(x, y, z);
            var maxY = Mathf.Max(contraption.Position.y, y) + 32;
            seed.Velocity = VanillaProjectileExt.GetLobVelocity(contraption.Position, target, maxY, seed.GetGravity());

            return seed;
        }
        public const float EXPLOSION_RADIUS = 40;
        public const float EXPLOSION_DAMAGE = 1800;
    }
}

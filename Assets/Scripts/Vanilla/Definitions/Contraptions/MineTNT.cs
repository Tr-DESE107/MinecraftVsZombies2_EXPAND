using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.mineTNT)]
    [EntitySeedDefinition(25, VanillaMod.spaceName, RechargeNames.longTime)]
    public class MineTNT : VanillaContraption
    {
        public MineTNT(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SHELL, ShellID.grass);
            SetProperty(EntityProps.PLACE_SOUND, SoundID.grass);
            SetProperty(EntityProps.DEATH_SOUND, SoundID.grass);
            SetProperty(EntityProperties.SIZE, new Vector3(48, 24, 48));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var riseTimer = new FrameTimer(450);
            SetRiseTimer(entity, riseTimer);

            entity.CollisionMask |= EntityCollision.MASK_ENEMY | EntityCollision.MASK_HOSTILE;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            RiseUpdate(entity);
        }

        public override void Evoke(Entity entity)
        {
            base.Evoke(entity);
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
                entity.Level.PlaySound(SoundID.dirtRise, entity.Pos);
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
            entity.Level.Explode(entity.Pos, EXPLOSION_RADIUS, entity.GetFaction(), 1800, new DamageEffectList(DamageEffects.MUTE, DamageFlags.IGNORE_ARMOR, DamageEffects.REMOVE_ON_DEATH), new EntityReferenceChain(entity));
            entity.Level.Spawn<MineDebris>(entity.Pos, entity);
            entity.Remove();
            entity.Level.PlaySound(SoundID.mineExplode, entity.Pos);
            entity.Level.ShakeScreen(10, 0, 15);
        }
        public const float EXPLOSION_RADIUS = 40;
        public const float EXPLOSION_DAMAGE = 1800;
    }
}

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.hellChariot)]
    public class HellChariot : StateEnemy
    {
        public HellChariot(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetPunctureTimer(entity, new FrameTimer(PUNCTURE_TIME));
            if (!entity.IsPreviewEnemy())
            {
                entity.PlaySound(VanillaSoundID.trainWhistle);
                entity.Level.AddLoopSoundEntity(VanillaSoundID.trainTravel, entity.ID);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            bool broken = entity.Health <= BROKEN_THRESOLD;
            if (!entity.IsDead && broken)
            {
                entity.Health -= 2;
                if (entity.Health <= 0)
                {
                    entity.Die();
                }
            }
            // 设置血量状态。
            bool punctured = IsPunctured(entity);
            var hp = entity.Health;
            if (punctured)
            {
                var timer = GetPunctureTimer(entity);
                timer.Run();
                if (timer.Expired)
                {
                    entity.Die();
                }
                else
                {
                    hp *= timer.Frame / (float)timer.MaxFrame;
                    entity.PlaySound(VanillaSoundID.shieldHit);
                }
            }
            entity.SetModelDamagePercent(hp, entity.GetMaxHealth());
            entity.SetAnimationBool("Shaking", broken || punctured);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            if (!collision.Collider.IsForMain())
                return;
            var other = collision.Other;
            if (other.IsDead || !other.IsVulnerableEntity())
                return;
            var chariot = collision.Entity;
            if (IsPunctured(chariot))
                return;
            if (!chariot.IsHostile(other))
                return;

            bool blocked = false;
            float damage = other.GetTakenCrushDamage();
            var vehicleInteraction = other.GetVehicleInteraction();
            if (vehicleInteraction == VehicleInteraction.BLOCK)
            {
                damage = chariot.GetDamage() * 0.1f;
            }
            var output = collision.OtherCollider.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), chariot);
            if (output != null)
            {
                if (output.BodyResult != null && output.BodyResult.Fatal && other.Type == EntityTypes.PLANT)
                {
                    other.PlaySound(VanillaSoundID.smash);
                }
            }
            if (other.IsInvincible() || vehicleInteraction == VehicleInteraction.BLOCK)
            {
                blocked = true;
            }

            if (blocked)
            {
                var vel = chariot.Velocity;
                if (vel.x * chariot.GetFacingX() > 0)
                {
                    vel.x = 0;
                }
                chariot.Velocity = vel;
            }
            if (vehicleInteraction == VehicleInteraction.SPIKES)
            {
                Puncture(chariot);
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.SIZE, entity.GetScaledSize());
            var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), param);

            var anubisandOffset = ANUBISAND_OFFSET;
            anubisandOffset.x *= entity.GetFacingX();
            var anubisand = entity.SpawnWithParams(VanillaEnemyID.anubisand, entity.Position + anubisandOffset);
            entity.Remove();
        }
        #endregion

        public static void Puncture(Entity entity)
        {
            if (IsPunctured(entity))
                return;
            SetPunctured(entity, true);
            var timer = GetPunctureTimer(entity);
            timer.Reset();
        }

        #region 字段
        public static bool IsPunctured(Entity entity) => entity.GetBehaviourField<bool>(ID, FIELD_PUNCTURED);
        public static void SetPunctured(Entity entity, bool value) => entity.SetBehaviourField(ID, FIELD_PUNCTURED, value);

        public static FrameTimer GetPunctureTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, FIELD_PUNCTURE_TIMER);
        public static void SetPunctureTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, FIELD_PUNCTURE_TIMER, value);
        #endregion

        public const float BROKEN_THRESOLD = 200;
        public static readonly Vector3 ANUBISAND_OFFSET = new Vector3(-48, 32, 0);
        public const int PUNCTURE_TIME = 40;
        private static readonly VanillaEntityPropertyMeta<bool> FIELD_PUNCTURED = new VanillaEntityPropertyMeta<bool>("Punctured");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> FIELD_PUNCTURE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("PunctureTimer");
        private static readonly NamespaceID ID = VanillaEnemyID.hellChariot;
    }
}

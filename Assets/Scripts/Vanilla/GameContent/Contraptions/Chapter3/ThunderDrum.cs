using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.thunderDrum)]
    public class ThunderDrum : ContraptionBehaviour
    {
        public ThunderDrum(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetRestoreTimer(entity, new FrameTimer(RESTORE_TIME));
            SetEvocationTimer(entity, new FrameTimer(EVOCATION_DURATION));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (entity.IsEvoked())
            {
                var evocationTimer = GetEvocationTimer(entity);
                evocationTimer.Run();

                detectBuffer.Clear();
                entity.Level.FindEntitiesNonAlloc(e => IsValidTarget(entity, e), detectBuffer);
                foreach (var target in detectBuffer)
                {
                    target.TakeDamage(target.GetMaxHealth() * TOTAL_HP_LOSS / (float)EVOCATION_DURATION, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.MUTE), entity);
                }
                entity.Level.ShakeScreen(3, 0, 3);

                if (evocationTimer.Expired)
                {
                    entity.SetEvoked(false);
                    entity.Level.RemoveLoopSoundEntity(VanillaSoundID.earthquake, entity.ID);
                }
            }
            else if (IsBroken(entity))
            {
                var restoreTimer = GetRestoreTimer(entity);
                restoreTimer.Run(entity.GetAttackSpeed());
                if (restoreTimer.Expired)
                {
                    SetBroken(entity, false);
                }
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationBool("Broken", IsBroken(entity));
        }
        public override bool CanTrigger(Entity entity)
        {
            return base.CanTrigger(entity) && !entity.IsEvoked() && !IsBroken(entity);
        }
        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            Quake(entity);
            SetBroken(entity, true);
            var restoreTimer = GetRestoreTimer(entity);
            restoreTimer.Reset();
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            SetBroken(entity, false);

            entity.Level.AddLoopSoundEntity(VanillaSoundID.earthquake, entity.ID);

            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.ResetTime(EVOCATION_DURATION);
        }
        private bool IsValidTarget(Entity self, Entity target)
        {
            if (!self.IsHostile(target))
                return false;
            if (target.IsDead)
                return false;
            if (!target.IsOnGround)
                return false;
            if (target.IsOnWater())
                return false;
            return true;
        }
        private void Quake(Entity self)
        {
            detectBuffer.Clear();
            self.Level.FindEntitiesNonAlloc(e => IsValidTarget(self, e), detectBuffer);
            foreach (var target in detectBuffer)
            {
                if (target.Type != EntityTypes.ENEMY)
                    continue;
                var vel = target.Velocity;
                vel.x = 4;
                vel.y = 15;
                target.Velocity = vel;
                target.RandomChangeAdjacentLane(self.RNG);
            }
            self.Level.ShakeScreen(15, 0, 30);
            self.PlaySound(VanillaSoundID.lightningAttack);
        }
        public static bool IsBroken(Entity entity) => entity.GetBehaviourField<bool>(ID, FIELD_BROKEN);
        public static void SetBroken(Entity entity, bool value) => entity.SetBehaviourField(ID, FIELD_BROKEN, value);
        public static FrameTimer GetRestoreTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, FIELD_RESTORE_TIMER);
        public static void SetRestoreTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, FIELD_RESTORE_TIMER, timer);
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, FIELD_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, FIELD_EVOCATION_TIMER, timer);

        public const int RESTORE_TIME = 600;
        public const int EVOCATION_DURATION = 120;
        public const float TOTAL_HP_LOSS = 0.25f;
        public const string FIELD_BROKEN = "Broken";
        public const string FIELD_RESTORE_TIMER = "RestoreTimer";
        public const string FIELD_EVOCATION_TIMER = "EvocationTimer";
        private static readonly NamespaceID ID = VanillaContraptionID.thunderDrum;
        private List<Entity> detectBuffer = new List<Entity>();
    }
}

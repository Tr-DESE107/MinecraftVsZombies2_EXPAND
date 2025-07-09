﻿using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.hfpd)]
    public class HighFrequencyPulseDispenser : DispenserFamily
    {
        public HighFrequencyPulseDispenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetEvocationTimer(entity, new FrameTimer(EVOCATION_TIME));
            SetRepeatTimer(entity, new FrameTimer(REPEAT_INVERVAL));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                int repeatCount = GetRepeatCount(entity);
                if (repeatCount > 0)
                {
                    var repeatTimer = GetRepeatTimer(entity);
                    repeatTimer.Run(entity.GetAttackSpeed());
                    if (repeatTimer.Expired)
                    {
                        Shoot(entity);
                        SetRepeatCount(entity, repeatCount - 1);
                        repeatTimer.Reset();
                    }
                }
            }
            else
            {
                EvokedUpdate(entity);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("Upgraded", IsUpgraded(entity));
            entity.SetModelProperty("GatlinAlt", IsGatlinAlt(entity));
        }
        public override void OnShootTick(Entity entity)
        {
            int count = IsUpgraded(entity) ? REPEAT_COUNT_UPGRADED : REPEAT_COUNT;
            SetRepeatCount(entity, count);
            var repeatTimer = GetRepeatTimer(entity);
            repeatTimer.ResetTime(Mathf.FloorToInt(15f / count));
            repeatTimer.Frame = 0;
        }
        public override Entity Shoot(Entity entity)
        {
            SetGatlinAlt(entity, !IsGatlinAlt(entity));
            return base.Shoot(entity);
        }

        public override bool CanEvoke(Entity entity)
        {
            if (IsUpgraded(entity))
                return false;
            return base.CanEvoke(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.AddBuff<HFPDUpgradedBuff>();
            //var evocationTimer = GetEvocationTimer(entity);
            //evocationTimer.Reset();
            //entity.SetEvoked(true);
            entity.PlaySound(VanillaSoundID.gunReload);
            entity.PlaySound(VanillaSoundID.powerUp);
            entity.PlaySound(VanillaSoundID.motor);
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.PassedInterval(15))
            {
                var projectile = Shoot(entity);
                projectile.Velocity *= 2;
            }
            if (evocationTimer.Expired)
            {
                ShootLargeArrow(entity);
                entity.SetEvoked(false);
                var shootTimer = GetShootTimer(entity);
                shootTimer.Reset();
            }
        }
        private Entity ShootLargeArrow(Entity entity)
        {
            entity.TriggerAnimation("Shoot");

            var param = entity.GetShootParams();

            var offset = entity.GetShotOffset();
            offset = entity.ModifyShotOffset(offset);
            param.position = entity.Position + offset;
            param.velocity = param.velocity.normalized;

            param.projectileID = VanillaProjectileID.largeArrow;
            param.damage = entity.GetDamage() * 30;
            param.soundID = VanillaSoundID.spellCard;

            return entity.ShootProjectile(param);
        }
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EVOCATION_TIMER, timer);
        public static FrameTimer GetRepeatTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_REPEAT_TIMER);
        public static void SetRepeatTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_REPEAT_TIMER, timer);

        public static bool IsGatlinAlt(Entity entity) => entity.GetBehaviourField<bool>(PROP_GATLIN_ALT);
        public static void SetGatlinAlt(Entity entity, bool value) => entity.SetBehaviourField(PROP_GATLIN_ALT, value);
        public static bool IsUpgraded(Entity entity) => entity.HasBuff<HFPDUpgradedBuff>();
        public static int GetRepeatCount(Entity entity) => entity.GetBehaviourField<int>(PROP_REPEAT_COUNT);
        public static void SetRepeatCount(Entity entity, int count) => entity.SetBehaviourField(PROP_REPEAT_COUNT, count);

        public const int REPEAT_INVERVAL = 7;
        public const int EVOCATION_TIME = 60;
        public const int REPEAT_COUNT = 4;
        public const int REPEAT_COUNT_UPGRADED = 6;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_REPEAT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("RepeatTimer");
        public static readonly VanillaEntityPropertyMeta<int> PROP_REPEAT_COUNT = new VanillaEntityPropertyMeta<int>("RepeatCount");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_GATLIN_ALT = new VanillaEntityPropertyMeta<bool>("gatlin_alt");
    }
}

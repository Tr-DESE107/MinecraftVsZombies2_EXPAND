﻿using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.drivenser)]
    public class Drivenser : DispenserFamily
    {
        public Drivenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetRepeatTimer(entity, new FrameTimer(15));
            if (entity.Level.IsIZombie())
            {
                SetUpgradeLevel(entity, I_ZOMBIE_LEVEL);
            }
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
                var shootTimer = GetShootTimer(entity);
                shootTimer.Run();
                if (shootTimer.Expired)
                {
                    entity.TriggerAnimation("Shoot");
                    var shootParams = entity.GetShootParams();
                    shootParams.projectileID = VanillaProjectileID.largeArrow;
                    shootParams.damage = entity.GetDamage() * 30;
                    shootParams.soundID = VanillaSoundID.spellCard;
                    shootParams.velocity = shootParams.velocity.normalized;
                    entity.ShootProjectile(shootParams);

                    int repeatCount = GetRepeatCount(entity);
                    repeatCount--;
                    SetRepeatCount(entity, repeatCount);
                    if (repeatCount <= 0)
                    {
                        entity.SetEvoked(false);
                    }
                    shootTimer.ResetTime(GetTimerTime(entity));
                }
            }
            var blockerBlend = GetBlockerBlend(entity);
            blockerBlend = blockerBlend * 0.5f + GetUpgradeLevel(entity) * 0.5f;
            SetBlockerBlend(entity, blockerBlend);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            var blend = GetBlockerBlend(entity);
            entity.SetModelProperty("Level", GetUpgradeLevel(entity));
            entity.SetAnimationFloat("BlockerBlend", blend);
            entity.SetAnimationFloat("RotateSpeed", blend + 1);
        }
        public override void OnShootTick(Entity entity)
        {
            int count = GetUpgradeLevel(entity) * 2 + 2;
            SetRepeatCount(entity, count);
            var repeatTimer = GetRepeatTimer(entity);
            repeatTimer.ResetTime(Mathf.FloorToInt(15f / count));
            repeatTimer.Frame = 0;
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            SetRepeatCount(entity, 5);
        }
        public static FrameTimer GetRepeatTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_REPEAT_TIMER);
        public static void SetRepeatTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_REPEAT_TIMER, timer);

        public static int GetRepeatCount(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_REPEAT_COUNT);
        public static void SetRepeatCount(Entity entity, int count) => entity.SetBehaviourField(ID, PROP_REPEAT_COUNT, count);

        public static int GetUpgradeLevel(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_UPGRADE_LEVEL);
        public static void SetUpgradeLevel(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_UPGRADE_LEVEL, value);

        public static float GetBlockerBlend(Entity entity) => entity.GetBehaviourField<float>(ID, PROP_BLOCKER_BLEND);
        public static void SetBlockerBlend(Entity entity, float value) => entity.SetBehaviourField(ID, PROP_BLOCKER_BLEND, value);

        public static bool CanUpgrade(Entity drivenser)
        {
            return drivenser.IsEntityOf(VanillaContraptionID.drivenser) && drivenser.IsFriendlyEntity() && GetUpgradeLevel(drivenser) < MAX_UPGRADE_LEVEL;
        }
        public static void Upgrade(Entity drivenser)
        {
            if (!drivenser.IsEntityOf(VanillaContraptionID.drivenser))
                return;
            SetUpgradeLevel(drivenser, GetUpgradeLevel(drivenser) + 1);
            drivenser.PlaySound(VanillaSoundID.mechanism);
            drivenser.Level.Spawn(VanillaEffectID.gearParticles, drivenser.Position, drivenser);
        }
        public const int MAX_UPGRADE_LEVEL = 4;
        public const int I_ZOMBIE_LEVEL = 2;
        private static readonly NamespaceID ID = VanillaContraptionID.drivenser;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_REPEAT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("RepeatTimer");
        public static readonly VanillaEntityPropertyMeta<int> PROP_REPEAT_COUNT = new VanillaEntityPropertyMeta<int>("RepeatCount");
        public static readonly VanillaEntityPropertyMeta<int> PROP_UPGRADE_LEVEL = new VanillaEntityPropertyMeta<int>("UpgradeLevel");
        public static readonly VanillaEntityPropertyMeta<float> PROP_BLOCKER_BLEND = new VanillaEntityPropertyMeta<float>("BlockerBlend");
    }
}

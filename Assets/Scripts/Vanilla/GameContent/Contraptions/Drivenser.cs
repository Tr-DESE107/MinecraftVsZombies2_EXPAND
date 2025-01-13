using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Triggers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.drivenser)]
    public class Drivenser : DispenserFamily, IStackEntity
    {
        public Drivenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetRepeatTimer(entity, new FrameTimer(15));
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
                    shootParams.damage = entity.GetDamage() * 50;
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
            entity.SetAnimationInt("Level", GetUpgradeLevel(entity));
            entity.SetAnimationFloat("BlockerBlend", blend);
            entity.SetAnimationFloat("RotateSpeed", blend + 1);
        }
        public override void OnShootTick(Entity entity)
        {
            int count = GetUpgradeLevel(entity) + 1;
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
        public static FrameTimer GetRepeatTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, "RepeatTimer");
        public static void SetRepeatTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, "RepeatTimer", timer);

        public static int GetRepeatCount(Entity entity) => entity.GetBehaviourField<int>(ID, "RepeatCount");
        public static void SetRepeatCount(Entity entity, int count) => entity.SetBehaviourField(ID, "RepeatCount", count);

        public static int GetUpgradeLevel(Entity entity) => entity.GetBehaviourField<int>(ID, "UpgradeLevel");
        public static void SetUpgradeLevel(Entity entity, int value) => entity.SetBehaviourField(ID, "UpgradeLevel", value);

        public static float GetBlockerBlend(Entity entity) => entity.GetBehaviourField<float>(ID, "BlockerBlend");
        public static void SetBlockerBlend(Entity entity, float value) => entity.SetBehaviourField(ID, "BlockerBlend", value);


        void IStackEntity.CanStackOnEntity(Entity target, TriggerResultBoolean result)
        {
            if (target.IsEntityOf(VanillaContraptionID.drivenser) && GetUpgradeLevel(target) < MAX_UPGRADE_LEVEL)
            {
                result.Result = true;
            }
        }
        void IStackEntity.StackOnEntity(Entity target)
        {
            if (!target.IsEntityOf(VanillaContraptionID.drivenser))
                return;
            SetUpgradeLevel(target, GetUpgradeLevel(target) + 1);
            target.PlaySound(VanillaSoundID.mechanism);
            target.Level.Spawn(VanillaEffectID.gearParticles, target.Position, target);

        }
        public const int MAX_UPGRADE_LEVEL = 4;
        private static readonly NamespaceID ID = VanillaContraptionID.drivenser;
    }
}

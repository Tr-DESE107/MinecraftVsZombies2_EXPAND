﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.ghast)]
    public class Ghast : StateEnemy
    {
        public Ghast(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true
            };
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(SHOOT_COOLDOWN));
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 80f);
        }
        protected override void UpdateAI(Entity enemy)
        {
            if (CanShoot(enemy))
            {
                var shootTimer = GetStateTimer(enemy);
                shootTimer.Run(enemy.GetAttackSpeed());
                switch (enemy.State)
                {
                    case VanillaEntityStates.WALK:
                        if (shootTimer.Expired)
                        {
                            var target = FindTarget(enemy);
                            if (target != null && target.Exists())
                            {
                                enemy.Target = target;
                                shootTimer.ResetTime(SHOOT_DURATION);
                                enemy.PlayCrySound(VanillaSoundID.ghastFire);
                            }
                            else
                            {
                                shootTimer.Reset();
                            }
                        }
                        break;
                    case VanillaEntityStates.ATTACK:
                        if (shootTimer.Expired)
                        {
                            var target = FindTarget(enemy);
                            if (target != null && target.Exists())
                            {
                                Fire(enemy, target);
                            }
                            enemy.Target = null;
                            shootTimer.ResetTime(SHOOT_COOLDOWN);
                        }
                        break;
                }
            }
            base.UpdateAI(enemy);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Source.DefinitionID == VanillaProjectileID.fireCharge && !entity.Level.IsIZombie())
            {
                Global.Game.Unlock(VanillaUnlockID.returnToSender);
            }
        }
        public static FrameTimer GetStateTimer(Entity enemy)
        {
            return enemy.GetBehaviourField<FrameTimer>(ID, PROP_STATE_TIMER);
        }
        public static void SetStateTimer(Entity enemy, FrameTimer value)
        {
            enemy.SetBehaviourField(ID, PROP_STATE_TIMER, value);
        }
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);
            WalkUpdate(enemy);
        }
        protected virtual bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }
        protected virtual Entity FindTarget(Entity entity)
        {
            return detector.DetectEntityWithTheLeast(entity, e => (e.GetCenter() - entity.Position).sqrMagnitude);
        }
        protected virtual bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }
        private void Fire(Entity self, Entity target)
        {
            var scale = self.GetScale();

            var param = self.GetShootParams();
            var shootPoint = self.GetShootPoint();
            var velocity = self.GetShotVelocity();
            var speed = velocity.magnitude;
            var direciton = (target.GetCenter() - shootPoint).normalized;
            param.velocity = speed * direciton;
            var damageMultiplier = self.Level.GetGhastDamageMultiplier();
            param.damage = self.GetDamage() * damageMultiplier;

            var bullet = self.ShootProjectile(param);
            var buff = bullet.AddBuff<GhastFireChargeBuff>();
            GhastFireChargeBuff.SetScaleMultiplier(buff, scale);
            GhastFireChargeBuff.SetRangeMultiplier(buff, scale.x);
            GhastFireChargeBuff.SetDamageMultiplier(buff, scale.x);
            self.PlaySound(VanillaSoundID.fireCharge, scale.x);
        }
        private Detector detector;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateTimer");
        public const int SHOOT_COOLDOWN = 135;
        public const int SHOOT_DURATION = 15;
        public static readonly NamespaceID ID = VanillaEnemyID.ghast;
    }
}

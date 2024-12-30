using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    [Definition(VanillaBossNames.frankenstein)]
    public class Frankenstein : BossBehaviour
    {
        public Frankenstein(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            SetStateTimer(boss, new FrameTimer(150));
            SetSubStateTimer(boss, new FrameTimer());
            SetDetectTimer(boss, new FrameTimer());
            SetShockRNG(boss, new RandomGenerator(boss.RNG.Next()));
            SetBulletRNG(boss, new RandomGenerator(boss.RNG.Next()));
            SetJumpRNG(boss, new RandomGenerator(boss.RNG.Next()));

            StartState(boss, STATE_WAKING);

            if (boss.Level.Difficulty == VanillaDifficulties.hard)
            {
                EnterSteelPhase(boss);
            }
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            if (CanTransformPhase(entity) && entity.State != STATE_FAINT)
            {
                StartState(entity, STATE_FAINT);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(60);
                entity.PlaySound(VanillaSoundID.powerOff);
            }
            var stateMachine = GetStateMachine(entity.State);
            stateMachine.OnUpdate(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            UpdateAim(entity);
            entity.SetAnimationBool("MissileVisible", entity.State == STATE_MISSILE && GetSubState(entity) == SUBSTATE_MISSILE_AIM);

            if (entity.State == STATE_DEAD)
            {
                var stateMachine = GetStateMachine(entity.State);
                stateMachine.OnUpdate(entity);
            }
            entity.SetAnimationFloat("ActionSpeed", GetActionSpeed(entity));
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            StartState(boss, STATE_DEAD);
        }
        public override void PreTakeDamage(DamageInput damageInfo)
        {
            base.PreTakeDamage(damageInfo);
            if (damageInfo.Amount > 600)
            {
                damageInfo.SetAmount(600);
            }
        }
        #endregion 事件

        private static void EnterSteelPhase(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.bloody);
            var level = boss.Level;
            var center = boss.Position + new Vector3(-50, 80, 0);

            level.Spawn(VanillaEffectID.gore, center, boss);

            var bloodPart = level.Spawn(VanillaEffectID.bloodParticles, center, boss);
            var bloodColor = boss.GetBloodColor();
            bloodPart.SetTint(bloodColor);

            SetSteelPhase(boss, true);
        }
        private static void StartState(Entity boss, int state)
        {
            SetSubState(boss, 0);
            boss.State = state;
            if (state == STATE_WAKING || state == STATE_FAINT && !IsParalyzed(boss))
            {
                boss.AddBuff<FrankensteinTransformingBuff>();
            }
            else
            {
                boss.RemoveBuffs(boss.GetBuffs<FrankensteinTransformingBuff>());
            }
            boss.SetAnimationInt("State", (int)state);
            boss.SetAnimationBool("EyelightStable", state != STATE_FAINT && state != STATE_DEAD);

            var substateTimer = GetSubStateTimer(boss);
            var nextStateTimer = GetStateTimer(boss);
            var detectTimer = GetDetectTimer(boss);
            substateTimer.Stop();
            nextStateTimer.Stop();
            detectTimer.Stop();

            var stateMachine = GetStateMachine(state);
            stateMachine.OnEnter(boss);
        }
        private void UpdateAim(Entity boss)
        {
            Entity target = boss.Target;
            var substate = GetSubState(boss);
            // 内手臂，发射子弹
            Vector3 innerArmRootPosition = boss.Position + innerArmRootOffset;

            Vector3 innerDir = boss.GetFacingDirection();
            if (boss.State == STATE_GUN && target != null)
            {
                innerDir = (target.GetCenter() - innerArmRootPosition).normalized;
            }
            var gunDir = GetGunDirection(boss);
            SetGunDirection(boss, Vector3.Lerp(gunDir, innerDir, 0.3f));

            // 外手臂，发射火箭
            Vector3 outerArmRootPosition = boss.Position + innerArmRootOffset;

            Vector3 outerDir = boss.GetFacingDirection();
            if (boss.State == STATE_MISSILE && substate == SUBSTATE_MISSILE_AIM && target != null)
            {
                outerDir = (target.GetCenter() - outerArmRootPosition).normalized;
            }
            var missileDir = GetMissileDirection(boss);
            SetMissileDirection(boss, Vector3.Lerp(missileDir, outerDir, 0.3f));
        }
        private static Entity FindMissileTarget(Entity boss)
        {
            return missileDetector.DetectOrderBy(boss, e => e.Position.x - boss.Position.x);
        }
        private static Entity FindPunchTarget(Entity boss)
        {
            return boss.Level.FindFirstEntity(e => IsPunchable(boss, e));
        }

        /// <summary>
        /// 寻找机枪目标。
        /// </summary>
        private static Entity FindGunTarget(Entity entity)
        {
            var level = entity.Level;
            return gunDetector.DetectOrderBy(entity, t => entity.Position.x - t.Position.x);
        }
        private static Entity FindShockingTarget(Entity boss)
        {
            return boss.Level.FindFirstEntity(e => IsShockable(boss, e));
        }
        private static bool IsPunchable(Entity boss, Entity target)
        {
            if (!target.IsVulnerableEntity())
                return false;
            return boss.IsHostile(target) && Detection.IsInFrontOf(boss, target, 20, 80) && target.GetLane() == boss.GetLane();
        }
        private static bool IsShockable(Entity boss, Entity target)
        {
            if (target.IsEntityOf(VanillaContraptionID.tnt))
                return true;
            return target.Type == EntityTypes.PLANT && boss.IsHostile(target) && Detection.IsInFrontOf(boss, target, 20) && target.CanShock();
        }

        public static void DoTransformationEffects(Entity boss)
        {
            // 震动特效
            Vector3 centerPos = boss.Position + awakeOffset;

            var level = boss.Level;
            level.Thunder();
            boss.PlaySound(VanillaSoundID.thunder);
            boss.PlaySound(VanillaSoundID.smash);
            level.Spawn(VanillaEffectID.thunderBolt, centerPos, boss);

            var explosion = level.Spawn(VanillaEffectID.explosion, centerPos, boss);
            explosion.SetSize(Vector3.one * 120);

            const int arcCounts = 8;
            const float arcAngle = 360 / arcCounts;

            for (int i = 0; i < arcCounts; i++)
            {
                float rad = i * arcAngle * Mathf.Deg2Rad;

                var arc = level.Spawn(VanillaEffectID.electricArc, centerPos + Vector3.up, boss);
                Vector3 arcTargetPos = centerPos + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * 100;
                ElectricArc.Connect(arc, arcTargetPos);
            }

            level.ShakeScreen(20, 0, 15);
        }
        public static void Paralyze(Entity boss, Entity source)
        {
            boss.TakeDamage(1200, new DamageEffectList(VanillaDamageEffects.LIGHTNING), source);
            if (!boss.IsDead)
            {
                SetParalyzed(boss, true);
                StartState(boss, STATE_FAINT);
                var substateTimer = GetSubStateTimer(boss);
                substateTimer.ResetTime(300);
                boss.PlaySound(VanillaSoundID.powerOff);
            }
        }
        private static StateMachine GetStateMachine(int state)
        {
            if (stateMachines.TryGetValue(state, out var machine))
            {
                return machine;
            }
            return stateMachines[STATE_IDLE];
        }
        private static bool CanTransformPhase(Entity boss)
        {
            if (boss.Level.Difficulty == VanillaDifficulties.easy)
                return false;
            return boss.Health <= boss.GetMaxHealth() * 0.5f && !IsSteelPhase(boss);
        }

        public static float GetActionSpeed(Entity boss)
        {
            return boss.Level.Difficulty == VanillaDifficulties.hard ? 2 : 1;
        }

        #region 属性
        public static int GetLastState(Entity boss)
        {
            return boss.GetBehaviourProperty<int>(ID, "LastState");
        }
        public static void SetLastState(Entity boss, int value)
        {
            boss.SetBehaviourProperty(ID, "LastState", value);
        }

        public static bool IsParalyzed(Entity boss)
        {
            return boss.GetBehaviourProperty<bool>(ID, "Paralyzed");
        }
        public static void SetParalyzed(Entity boss, bool value)
        {
            boss.SetBehaviourProperty(ID, "Paralyzed", value);
        }

        public static bool IsSteelPhase(Entity boss)
        {
            return boss.GetBehaviourProperty<bool>(ID, PROP_STEEL_PHASE);
        }
        public static void SetSteelPhase(Entity boss, bool value)
        {
            boss.SetBehaviourProperty(ID, PROP_STEEL_PHASE, value);
            if (value)
            {
                boss.AddBuff<FrankensteinSteelBuff>();
            }
            else
            {
                boss.RemoveBuffs(boss.GetBuffs<FrankensteinSteelBuff>());
            }
            boss.SetAnimationBool("Steel", value);
        }

        public static int GetSubState(Entity boss)
        {
            return boss.GetBehaviourProperty<int>(ID, "SubState");
        }
        private static void SetSubState(Entity boss, int value)
        {
            boss.SetBehaviourProperty(ID, "SubState", value);
            boss.SetAnimationInt("SubState", value);
        }

        public static FrameTimer GetStateTimer(Entity boss)
        {
            return boss.GetBehaviourProperty<FrameTimer>(ID, PROP_STATE_TIMER);
        }
        private static void SetStateTimer(Entity boss, FrameTimer value)
        {
            boss.SetBehaviourProperty(ID, PROP_STATE_TIMER, value);
        }

        public static FrameTimer GetSubStateTimer(Entity boss)
        {
            return boss.GetBehaviourProperty<FrameTimer>(ID, PROP_SUBSTATE_TIMER);
        }
        private static void SetSubStateTimer(Entity boss, FrameTimer value)
        {
            boss.SetBehaviourProperty(ID, PROP_SUBSTATE_TIMER, value);
        }

        public static FrameTimer GetDetectTimer(Entity boss)
        {
            return boss.GetBehaviourProperty<FrameTimer>(ID, PROP_DETECT_TIMER);
        }
        private static void SetDetectTimer(Entity boss, FrameTimer value)
        {
            boss.SetBehaviourProperty(ID, PROP_DETECT_TIMER, value);
        }

        public static RandomGenerator GetShockRNG(Entity boss)
        {
            return boss.GetBehaviourProperty<RandomGenerator>(ID, PROP_SHOCK_RNG);
        }
        private static void SetShockRNG(Entity boss, RandomGenerator value)
        {
            boss.SetBehaviourProperty(ID, PROP_SHOCK_RNG, value);
        }

        public static RandomGenerator GetJumpRNG(Entity boss)
        {
            return boss.GetBehaviourProperty<RandomGenerator>(ID, PROP_JUMP_RNG);
        }
        private static void SetJumpRNG(Entity boss, RandomGenerator value)
        {
            boss.SetBehaviourProperty(ID, PROP_JUMP_RNG, value);
        }

        public static RandomGenerator GetBulletRNG(Entity boss)
        {
            return boss.GetBehaviourProperty<RandomGenerator>(ID, PROP_BULLET_RNG);
        }
        private static void SetBulletRNG(Entity boss, RandomGenerator value)
        {
            boss.SetBehaviourProperty(ID, PROP_BULLET_RNG, value);
        }

        private static Vector3 GetGunDirection(Entity boss)
        {
            return boss.GetBehaviourProperty<Vector3>(ID, PROP_GUN_DIRECTION);
        }
        private static void SetGunDirection(Entity boss, Vector3 direction)
        {
            boss.SetBehaviourProperty(ID, PROP_GUN_DIRECTION, direction);
            float innerAngle = Vector2.SignedAngle(boss.GetFacingDirection(), direction);
            boss.SetAnimationFloat("InnerArmAngle", innerAngle);
        }

        private static Vector3 GetMissileDirection(Entity boss)
        {
            return boss.GetBehaviourProperty<Vector3>(ID, PROP_MISSILE_DIRECTION);
        }
        private static void SetMissileDirection(Entity boss, Vector3 direction)
        {
            boss.SetBehaviourProperty(ID, PROP_MISSILE_DIRECTION, direction);
            float innerAngle = Vector2.SignedAngle(boss.GetFacingDirection(), direction);
            boss.SetAnimationFloat("OuterArmAngle", innerAngle);
        }

        private static Vector3 GetJumpTarget(Entity boss)
        {
            return boss.GetBehaviourProperty<Vector3>(ID, PROP_JUMP_TARGET);
        }
        private static void SetJumpTarget(Entity boss, Vector3 target)
        {
            boss.SetBehaviourProperty(ID, PROP_JUMP_TARGET, target);
        }

        #endregion 属性

        #region 常量
        public static readonly NamespaceID ID = VanillaBossID.frankenstein;
        private static readonly Dictionary<int, StateMachine> stateMachines = new Dictionary<int, StateMachine>()
        {
            { STATE_IDLE, new IdleState() },
            { STATE_JUMP, new JumpState() },
            { STATE_GUN, new GunState() },
            { STATE_DEAD, new DeadState() },
            { STATE_MISSILE, new MissileState() },
            { STATE_PUNCH, new PunchState() },
            { STATE_SHOCK, new ShockState() },
            { STATE_WAKING, new AwakeState() },
            { STATE_FAINT, new FaintState() },
        };

        private const int detectIntervalFrames = 3;
        private const int shootingPeriodFrames = 60;
        private const float jumpFlyingTime = 15;
        //private const float bulletSpeed = 10;
        //private const float missileSpeed = 8;
        private const float upperArmLength = 60;
        private static readonly Vector3 innerArmRootOffset = new Vector3(-12, 150, 0);
        private static readonly Vector3 outerArmRootOffset = new Vector3(10, 134, 0);
        private static readonly Vector3 headOffset = new Vector3(-12.5f, 153, 0);
        private static readonly Vector3 awakeOffset = new Vector3(-50, 0, 0);
        private static Detector gunDetector = new FrankensteinGunDetector(VanillaProjectileID.bullet);
        private static Detector missileDetector = new FrankensteinGunDetector(VanillaProjectileID.missile);

        private const string PROP_JUMP_TARGET = "JumpTarget";
        private const string PROP_STEEL_PHASE = "SteelPhase";

        private const string PROP_GUN_DIRECTION = "GunDirection";
        private const string PROP_MISSILE_DIRECTION = "MissileDirection";

        private const string PROP_STATE_TIMER = "StateTimer";
        private const string PROP_SUBSTATE_TIMER = "SubStateTimer";
        private const string PROP_DETECT_TIMER = "DetectTimer";

        private const string PROP_SHOCK_RNG = "ShockRNG";
        private const string PROP_JUMP_RNG = "JumpRNG";
        private const string PROP_BULLET_RNG = "BulletRNG";

        private const int STATE_IDLE = VanillaEntityStates.FRANKENSTEIN_IDLE;
        private const int STATE_JUMP = VanillaEntityStates.FRANKENSTEIN_JUMP;
        private const int STATE_GUN = VanillaEntityStates.FRANKENSTEIN_GUN;
        private const int STATE_DEAD = VanillaEntityStates.FRANKENSTEIN_DEAD;
        private const int STATE_MISSILE = VanillaEntityStates.FRANKENSTEIN_MISSILE;
        private const int STATE_PUNCH = VanillaEntityStates.FRANKENSTEIN_PUNCH;
        private const int STATE_SHOCK = VanillaEntityStates.FRANKENSTEIN_SHOCK;
        private const int STATE_WAKING = VanillaEntityStates.FRANKENSTEIN_WAKING;
        private const int STATE_FAINT = VanillaEntityStates.FRANKENSTEIN_FAINT;

        private const int SUBSTATE_AWAKE_START = 0;
        private const int SUBSTATE_AWAKE_LAUGH = 1;
        private const int SUBSTATE_AWAKE_RISE = 2;

        private const int SUBSTATE_GUN_READY = 0;
        private const int SUBSTATE_GUN_FIRE = 1;

        private const int SUBSTATE_DEAD_STAND = 0;
        private const int SUBSTATE_DEAD_HEAD_DROPPED = 1;

        private const int SUBSTATE_MISSILE_AIM = 0;
        private const int SUBSTATE_MISSILE_FIRED = 1;

        private const int SUBSTATE_PUNCH_READY = 0;
        private const int SUBSTATE_PUNCH_FIRE = 1;
        private const int SUBSTATE_PUNCH_FINISHED = 2;

        private const int SUBSTATE_SHOCK_READY = 0;
        private const int SUBSTATE_SHOCK_FINISHED = 1;

        private const int SUBSTATE_JUMP_READY = 0;
        private const int SUBSTATE_JUMP_IN_AIR = 1;
        #endregion 常量

        #region 状态机
        private class IdleState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var stateTimer = GetStateTimer(entity);
                stateTimer.ResetTime(90);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var nextStateTimer = GetStateTimer(entity);
                nextStateTimer.Run(GetActionSpeed(entity));
                if (!nextStateTimer.Expired)
                    return;

                var lastState = GetLastState(entity);
                if (IsSteelPhase(entity))
                {
                    if (lastState == STATE_JUMP)
                    {
                        lastState = STATE_PUNCH;
                        entity.Target = FindPunchTarget(entity);
                        if (entity.Target != null)
                        {
                            StartState(entity, STATE_PUNCH);
                            SetLastState(entity, lastState);
                            return;
                        }
                    }

                    if (lastState == STATE_PUNCH)
                    {
                        lastState = STATE_SHOCK;
                        entity.Target = FindShockingTarget(entity);
                        if (entity.Target != null)
                        {
                            StartState(entity, STATE_SHOCK);
                            SetLastState(entity, lastState);
                            return;
                        }
                    }
                }
                if (lastState != STATE_GUN)
                {
                    lastState = STATE_GUN;
                    entity.Target = FindGunTarget(entity);
                    if (entity.Target != null)
                    {
                        StartState(entity, STATE_GUN);
                        SetLastState(entity, lastState);
                        return;
                    }
                }

                lastState = STATE_JUMP;
                StartState(entity, STATE_JUMP);
                SetLastState(entity, lastState);
            }
        }
        private class AwakeState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                SetParalyzed(entity, false);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.powerOn);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (!substateTimer.Expired)
                    return;

                var substate = GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_AWAKE_START:
                        entity.PlaySound(IsSteelPhase(entity) ? VanillaSoundID.frankensteinSteelLaugh : VanillaSoundID.frankensteinLaugh);
                        SetSubState(entity, SUBSTATE_AWAKE_LAUGH);
                        substateTimer.ResetTime(60);
                        break;

                    case SUBSTATE_AWAKE_LAUGH:
                        SetSubState(entity, SUBSTATE_AWAKE_RISE);
                        substateTimer.ResetTime(100);
                        break;

                    case SUBSTATE_AWAKE_RISE:
                        StartState(entity, STATE_IDLE);
                        break;
                }
            }
        }
        private class DeadState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);

                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(90);

                entity.SetAnimationBool("Sparks", true);
                entity.Level.AddLoopSoundEntity(VanillaSoundID.electricSpark, entity.ID);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run();
                if (substateTimer.Expired)
                {
                    var substate = GetSubState(entity);
                    switch (substate)
                    {
                        case SUBSTATE_DEAD_STAND:
                            DropHead(entity);
                            SetSubState(entity, SUBSTATE_DEAD_HEAD_DROPPED);
                            substateTimer.ResetTime(120);
                            break;
                        case SUBSTATE_DEAD_HEAD_DROPPED:
                            var level = entity.Level;
                            var explosion = level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
                            explosion.SetSize(Vector3.one * 240);
                            entity.PlaySound(VanillaSoundID.explosion);
                            level.ShakeScreen(10, 0, 15);
                            entity.Remove();
                            break;
                    }
                }
            }
            private void DropHead(Entity boss)
            {
                var level = boss.Level;
                Vector3 headPos = boss.Position + headOffset;
                var headEffect = level.Spawn(VanillaEffectID.frankensteinHead, headPos, boss);
                headEffect.Velocity += new Vector3(boss.GetFacingX() * 5, 1, 0);
                headEffect.SetDisplayScale(new Vector3(-boss.GetFacingX(), 1, 1));
                FrankensteinHead.SetSteelPhase(headEffect, IsSteelPhase(boss));

                level.ShakeScreen(5, 0, 15);
                boss.PlaySound(VanillaSoundID.explosion);
                boss.PlaySound(VanillaSoundID.powerOff);
                var expPart = level.Spawn(VanillaEffectID.explosion, headPos, boss);
                expPart.SetSize(Vector3.one * 60);
            }
        }
        private class FaintState : StateMachine
        {
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (substateTimer.Expired)
                {
                    if (CanTransformPhase(entity))
                    {
                        EnterSteelPhase(entity);
                        DoTransformationEffects(entity);
                    }
                    StartState(entity, STATE_WAKING);
                }
            }
        }
        private class GunState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Reset();
                SetGunDirection(entity, entity.GetFacingDirection());

                entity.PlaySound(VanillaSoundID.gunReload);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substate = GetSubState(entity);
                // 如果没有开火，计时器结束时开火。
                if (substate == SUBSTATE_GUN_READY)
                {
                    var substateTimer = GetSubStateTimer(entity);
                    substateTimer.Run(GetActionSpeed(entity));
                    if (substateTimer.Expired)
                    {
                        substate = SUBSTATE_GUN_FIRE;
                        substateTimer.ResetTime(shootingPeriodFrames);
                        SetSubState(entity, substate);
                    }
                }
                // 寻找机枪目标
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Run(GetActionSpeed(entity));
                if (detectTimer.Expired || entity.Target == null || !entity.Target.Exists())
                {
                    detectTimer.ResetTime(detectIntervalFrames);
                    entity.Target = FindGunTarget(entity);
                }

                substate = GetSubState(entity);
                // 如果有目标，并且正在开火，则发射子弹
                if (entity.Target != null && entity.Target.Exists())
                {
                    if (substate == SUBSTATE_GUN_FIRE)
                    {
                        ShootBullets(entity);
                    }
                }
                // 如果没有目标，结束开火。
                else
                {
                    EndFiringBullets(entity);
                }
            }
            /// <summary>
            /// 停止发射子弹。
            /// </summary>
            private void EndFiringBullets(Entity boss)
            {
                boss.Target = FindMissileTarget(boss);
                if (boss.Target != null)
                {
                    StartState(boss, STATE_MISSILE);
                }
                else
                {
                    StartState(boss, STATE_IDLE);
                }
            }

            /// <summary>
            /// 发射一颗子弹。
            /// </summary>
            private void ShootABullet(Entity boss)
            {
                Vector3 armRootPosition = boss.Position + innerArmRootOffset;

                var rng = GetBulletRNG(boss);
                var gunDirection = GetGunDirection(boss);
                gunDirection.x = gunDirection.x * rng.Next(95, 110) / 100;
                gunDirection.y = gunDirection.y * rng.Next(95, 110) / 100;
                gunDirection.z = gunDirection.z * rng.Next(95, 110) / 100;
                gunDirection = gunDirection.normalized;
                SetGunDirection(boss, gunDirection);

                Vector3 gunPosition = armRootPosition + gunDirection * upperArmLength;

                var bullet = boss.ShootProjectile(new ShootParams()
                {
                    projectileID = VanillaProjectileID.bullet,
                    position = gunPosition,
                    velocity = gunDirection * boss.GetShotVelocity().magnitude,
                    damage = boss.GetDamage() * 0.1f,
                    faction = boss.GetFaction(),
                    soundID = VanillaSoundID.gunShot,
                });
                boss.TriggerAnimation("GunFire");
                boss.TriggerModel("GunFire");
            }

            /// <summary>
            /// 持续发射子弹。
            /// </summary>
            private void ShootBullets(Entity boss)
            {
                var substateTimer = GetSubStateTimer(boss);
                substateTimer.Run(GetActionSpeed(boss));
                var intervalCount = substateTimer.PassedIntervalCount(1);
                for (int i = 0; i < intervalCount; i++)
                {
                    ShootABullet(boss);
                }
                if (substateTimer.Expired)
                {
                    EndFiringBullets(boss);
                }
            }
        }
        private class MissileState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var substateTimer = GetSubStateTimer(entity);
                var detectTimer = GetDetectTimer(entity);
                substateTimer.ResetTime(30);
                detectTimer.ResetTime(detectIntervalFrames);
                SetMissileDirection(entity, entity.GetFacingDirection());
                entity.PlaySound(VanillaSoundID.gunReload);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Run(GetActionSpeed(entity));
                if (detectTimer.Expired)
                {
                    detectTimer.ResetTime(detectIntervalFrames);
                    entity.Target = FindMissileTarget(entity);
                }
                if (entity.Target == null || !entity.Target.Exists())
                {
                    StartState(entity, STATE_IDLE);
                    return;
                }

                var substate = GetSubState(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (!substateTimer.Expired)
                    return;

                switch (substate)
                {
                    case SUBSTATE_MISSILE_AIM:
                        FireMissile(entity);
                        break;
                    case SUBSTATE_MISSILE_FIRED:
                        StartState(entity, STATE_IDLE);
                        break;
                }
            }

            private void FireMissile(Entity boss)
            {
                SetSubState(boss, SUBSTATE_MISSILE_FIRED);
                var substateTimer = GetSubStateTimer(boss);
                substateTimer.ResetTime(9);

                Vector3 armRootPosition = boss.Position + outerArmRootOffset;
                var missileDirection = GetMissileDirection(boss);
                Vector3 missilePosition = armRootPosition + missileDirection * 80f;
                float missileSpeed = boss.GetShotVelocity().magnitude * 0.8f;

                var missile = boss.ShootProjectile(new ShootParams()
                {
                    projectileID = VanillaProjectileID.missile,
                    position = missilePosition,
                    velocity = missileDirection * missileSpeed,
                    damage = boss.GetDamage() * 2,
                    faction = boss.GetFaction(),
                    soundID = VanillaSoundID.missile
                });
            }
        }
        private class JumpState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(24);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (substateTimer.Expired)
                {
                    var substate = GetSubState(entity);
                    switch (substate)
                    {
                        case SUBSTATE_JUMP_READY:
                            SetSubState(entity, SUBSTATE_JUMP_IN_AIR);
                            Jump(entity);
                            break;

                        case SUBSTATE_JUMP_IN_AIR:
                            Vector3 target = GetJumpTarget(entity);
                            var pos = entity.Position;
                            pos.x = pos.x * 0.8f + target.x * 0.2f;
                            pos.z = pos.z * 0.8f + target.z * 0.2f;
                            entity.Position = pos;

                            entity.Level.Spawn(VanillaEffectID.frankensteinJumpTrail, entity.GetCenter(), entity);
                            if (entity.GetRelativeY() <= 0)
                            {
                                Land(entity);
                            }
                            break;
                    }
                }

            }
            private void Jump(Entity boss)
            {
                var grid = SearchJumpPlace(boss);
                SetJumpTarget(boss, grid?.GetEntityPosition() ?? boss.Position);

                Vector3 velocity = boss.Velocity;
                velocity.y = boss.GetGravity() * jumpFlyingTime;
                boss.Velocity = velocity;
                boss.PlaySound(VanillaSoundID.thunder);
            }

            private void Land(Entity boss)
            {
                StartState(boss, STATE_IDLE);

                var level = boss.Level;
                var bossColumn = boss.GetColumn();
                var bossLane = boss.GetLane();
                foreach (Entity ent in level.GetEntities())
                {
                    if (!ent.IsVulnerableEntity() || !boss.IsHostile(ent) || ent.GetColumn() != bossColumn || ent.GetLane() != bossLane)
                        continue;
                    if (ent.Type == EntityTypes.PLANT)
                    {
                        var damageOutput = ent.TakeDamage(58115310, new DamageEffectList(VanillaDamageEffects.PUNCH), boss);
                        if (damageOutput?.BodyResult?.Fatal ?? false)
                        {
                            boss.PlaySound(VanillaSoundID.smash);
                        }
                    }
                    else
                    {
                        ent.TakeDamage(1800, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR), boss);
                    }
                }
                level.ShakeScreen(5, 0, 15);
                boss.PlaySound(VanillaSoundID.thump);
                if (IsSteelPhase(boss))
                {
                    boss.PlaySound(VanillaSoundID.anvil);
                }
            }

            private LawnGrid SearchJumpPlace(Entity boss)
            {
                var level = boss.Level;
                int maxColumn = level.GetMaxColumnCount();
                int maxLane = level.GetMaxLaneCount();

                int column = 0;
                int lane = 0;
                var laneEnemyGroups = level.GetEntities().Where(e => e.IsVulnerableEntity() && boss.IsHostile(e) && e.GetLane() >= 0 && e.GetLane() < maxLane && e.GetLane() != boss.GetLane()).GroupBy(e => e.GetLane());
                if (laneEnemyGroups.Count() == 0)
                {
                    if (boss.IsFacingLeft())
                    {
                        column = boss.RNG.Next(maxColumn - 3, maxColumn);
                    }
                    else
                    {
                        column = boss.RNG.Next(0, 3);
                    }
                    lane = boss.RNG.Next(0, maxLane);
                    return level.GetGrid(column, lane);
                }
                var maxEnemyCount = laneEnemyGroups.Max(g => g.Count());
                var maxCountGroups = laneEnemyGroups.Where(g => g.Count() == maxEnemyCount);
                var targetLaneGroup = maxCountGroups.Random(GetJumpRNG(boss));
                if (boss.IsFacingLeft())
                {
                    var target = targetLaneGroup.OrderByDescending(e => e.GetColumn()).FirstOrDefault();
                    column = Mathf.Clamp(target.GetColumn() + 1, maxColumn - 3, maxColumn - 1);
                    lane = target.GetLane();
                }
                else
                {
                    var target = targetLaneGroup.OrderBy(e => e.GetColumn()).FirstOrDefault();
                    column = Mathf.Clamp(target.GetColumn() - 1, 0, 2);
                    lane = target.GetLane();
                }
                return level.GetGrid(column, lane);
            }
        }
        private class PunchState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.teslaPower);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (!substateTimer.Expired)
                    return;
                var substate = GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_PUNCH_READY:
                        SetSubState(entity, SUBSTATE_PUNCH_FIRE);
                        substateTimer.ResetTime(3);
                        break;

                    case SUBSTATE_PUNCH_FIRE:
                        SetSubState(entity, SUBSTATE_PUNCH_FINISHED);
                        substateTimer.ResetTime(15);
                        Punch(entity);
                        break;

                    case SUBSTATE_PUNCH_FINISHED:
                        StartState(entity, STATE_IDLE);
                        break;
                }
            }
            private void Punch(Entity boss)
            {
                foreach (Entity ent in boss.Level.FindEntities(e => IsPunchable(boss, e)))
                {
                    if (ent.Type == EntityTypes.PLANT)
                    {
                        ent.TakeDamage(58115310, new DamageEffectList(VanillaDamageEffects.PUNCH), boss);
                    }
                    else
                    {
                        ent.TakeDamage(1800, new DamageEffectList(VanillaDamageEffects.PUNCH), boss);
                    }
                }
                boss.Level.ShakeScreen(5, 0, 15);
                boss.PlaySound(VanillaSoundID.teslaAttack);
                boss.PlaySound(VanillaSoundID.smash);
            }
        }
        private class ShockState : StateMachine
        {
            public override void OnEnter(Entity entity)
            {
                base.OnEnter(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.teslaPower);
            }
            public override void OnUpdate(Entity entity)
            {
                base.OnUpdate(entity);
                var substateTimer = GetSubStateTimer(entity);
                substateTimer.Run(GetActionSpeed(entity));
                if (!substateTimer.Expired)
                    return;
                var substate = GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_SHOCK_READY:
                        SetSubState(entity, SUBSTATE_SHOCK_FINISHED);
                        entity.Target = FindShockingTarget(entity);
                        if (entity.Target != null)
                        {
                            Shock(entity);
                        }
                        substateTimer.ResetTime(15);
                        break;

                    case SUBSTATE_SHOCK_FINISHED:
                        StartState(entity, STATE_IDLE);
                        break;
                }
            }

            private void Shock(Entity boss)
            {
                var level = boss.Level;
                var shockables = level.FindEntities(e => IsShockable(boss, e));
                var targetsID = shockables.Select(e => e.GetDefinitionID());
                if (targetsID.Count() <= 0)
                    return;

                var contrapId = targetsID.Random(GetShockRNG(boss));

                bool soundPlayed = false;
                //再次遍历可以电击的器械。
                foreach (Entity contraption in shockables)
                {
                    if (contraption.IsEntityOf(VanillaContraptionID.tnt))
                    {
                        contraption.AddBuff<TNTChargedBuff>();
                        var arc = level.Spawn(VanillaEffectID.electricArc, boss.Position + outerArmRootOffset + Vector3.left * 100, boss);
                        ElectricArc.Connect(arc, contraption.Position);
                    }
                    else if (contraption.IsEntityOf(contrapId))
                    {
                        var buff = contraption.AddBuff<FrankensteinShockedBuff>();
                        buff.SetProperty(FrankensteinShockedBuff.PROP_TIMEOUT, 150);
                        if (!soundPlayed)
                        {
                            contraption.PlaySound(VanillaSoundID.powerOff);
                            soundPlayed = true;
                        }
                        var arc = level.Spawn(VanillaEffectID.electricArc, boss.Position + outerArmRootOffset + Vector3.left * 100, boss);
                        ElectricArc.Connect(arc, contraption.Position);
                    }
                }
                boss.PlaySound(VanillaSoundID.teslaAttack);
            }

        }
        #endregion
    }

    public class FrankensteinGunDetector : Detector
    {
        public FrankensteinGunDetector(NamespaceID projectileID)
        {
            this.projectileID = projectileID;
        }
        public override bool IsInRange(Entity self, Entity target)
        {
            if (!Detection.IsInFrontOf(self, target))
                return false;
            if (!TargetInLawn(target))
                return false;

            var projectileDef = self.Level.Content.GetEntityDefinition(projectileID);
            var projectileSize = projectileDef.GetProperty<Vector3>(EngineEntityProps.SIZE);
            var targetSize = target.GetScaledSize();
            return Detection.IsZCoincide(self.Position.z, projectileSize.z, target.Position.z, targetSize.z);
        }
        private NamespaceID projectileID;
    }
    public abstract class StateMachine
    {
        public int id;
        public virtual void OnEnter(Entity entity) { }
        public virtual void OnUpdate(Entity entity) { }
        public virtual void OnExit(Entity entity) { }
    }
}

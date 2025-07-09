﻿using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    public partial class Frankenstein
    {
        #region 状态机
        private class FrankensteinStateMachine : EntityStateMachine
        {
            public FrankensteinStateMachine()
            {
                AddState(new IdleState());
                AddState(new JumpState());
                AddState(new GunState());
                AddState(new DeadState());
                AddState(new MissileState());
                AddState(new PunchState());
                AddState(new ShockState());
                AddState(new AwakeState());
                AddState(new FaintState());
            }
            public override float GetSpeed(Entity entity)
            {
                return GetFrankensteinActionSpeed(entity);
            }
            protected override void OnEnterState(Entity entity, int state)
            {
                base.OnEnterState(entity, state);
                if (state == STATE_WAKING || (state == STATE_FAINT && !IsParalyzed(entity)))
                {
                    entity.AddBuff<FrankensteinTransformingBuff>();
                }
                else
                {
                    entity.RemoveBuffs<FrankensteinTransformingBuff>();
                }
                entity.SetAnimationBool("EyelightStable", state != STATE_FAINT && state != STATE_DEAD);

                var detectTimer = GetDetectTimer(entity);
                detectTimer.Stop();
            }
        }
        #endregion

        #region 状态
        private class IdleState : EntityStateMachineState
        {
            public IdleState() : base(STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(90);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var nextStateTimer = stateMachine.GetStateTimer(entity);
                nextStateTimer.Run(stateMachine.GetSpeed(entity));
                if (!nextStateTimer.Expired)
                    return;

                var lastState = stateMachine.GetPreviousState(entity);
                if (IsSteelPhase(entity))
                {
                    if (lastState == STATE_JUMP)
                    {
                        lastState = STATE_PUNCH;
                        entity.Target = FindPunchTarget(entity);
                        if (entity.Target != null)
                        {
                            stateMachine.StartState(entity, lastState);
                            stateMachine.SetPreviousState(entity, lastState);
                            return;
                        }
                    }

                    if (lastState == STATE_PUNCH)
                    {
                        lastState = STATE_SHOCK;
                        entity.Target = FindShockingTarget(entity);
                        if (entity.Target != null)
                        {
                            stateMachine.StartState(entity, lastState);
                            stateMachine.SetPreviousState(entity, lastState);
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
                        stateMachine.StartState(entity, lastState);
                        stateMachine.SetPreviousState(entity, lastState);
                        return;
                    }
                }

                lastState = STATE_JUMP;
                stateMachine.StartState(entity, lastState);
                stateMachine.SetPreviousState(entity, lastState);
            }
        }
        private class AwakeState : EntityStateMachineState
        {
            public AwakeState() : base(STATE_WAKING) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                SetParalyzed(entity, false);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.powerOn);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (!substateTimer.Expired)
                    return;

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_AWAKE_START:
                        entity.PlaySound(IsSteelPhase(entity) ? VanillaSoundID.frankensteinSteelLaugh : VanillaSoundID.frankensteinLaugh);
                        stateMachine.SetSubState(entity, SUBSTATE_AWAKE_LAUGH);
                        substateTimer.ResetTime(60);
                        break;

                    case SUBSTATE_AWAKE_LAUGH:
                        stateMachine.SetSubState(entity, SUBSTATE_AWAKE_RISE);
                        substateTimer.ResetTime(100);
                        break;

                    case SUBSTATE_AWAKE_RISE:
                        stateMachine.StartState(entity, STATE_IDLE);
                        break;
                }
            }
        }
        private class DeadState : EntityStateMachineState
        {
            public DeadState() : base(STATE_DEAD) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);

                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(90);

                entity.SetAnimationBool("Sparks", true);
                entity.Level.AddLoopSoundEntity(VanillaSoundID.electricSpark, entity.ID);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run();
                if (substateTimer.Expired)
                {
                    var substate = stateMachine.GetSubState(entity);
                    switch (substate)
                    {
                        case SUBSTATE_DEAD_STAND:
                            DropHead(entity);
                            stateMachine.SetSubState(entity, SUBSTATE_DEAD_HEAD_DROPPED);
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
        private class FaintState : EntityStateMachineState
        {
            public FaintState() : base(STATE_FAINT) { }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (substateTimer.Expired)
                {
                    if (CanTransformPhase(entity))
                    {
                        EnterSteelPhase(entity);
                        DoTransformationEffects(entity);
                    }
                    stateMachine.StartState(entity, STATE_WAKING);
                }
            }
        }
        private class GunState : EntityStateMachineState
        {
            public GunState() : base(STATE_GUN)
            {
            }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Reset();
                SetGunDirection(entity, entity.GetFacingDirection());

                entity.PlaySound(VanillaSoundID.gunReload);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substate = stateMachine.GetSubState(entity);
                // 如果没有开火，计时器结束时开火。
                if (substate == SUBSTATE_GUN_READY)
                {
                    var substateTimer = stateMachine.GetSubStateTimer(entity);
                    substateTimer.Run(stateMachine.GetSpeed(entity));
                    if (substateTimer.Expired)
                    {
                        substate = SUBSTATE_GUN_FIRE;
                        substateTimer.ResetTime(shootingPeriodFrames);
                        stateMachine.SetSubState(entity, substate);
                    }
                }
                // 寻找机枪目标
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Run(stateMachine.GetSpeed(entity));
                if (detectTimer.Expired || entity.Target == null || !entity.Target.Exists())
                {
                    detectTimer.ResetTime(detectIntervalFrames);
                    entity.Target = FindGunTarget(entity);
                }

                substate = stateMachine.GetSubState(entity);
                // 如果有目标，并且正在开火，则发射子弹
                if (entity.Target != null && entity.Target.Exists())
                {
                    if (substate == SUBSTATE_GUN_FIRE)
                    {
                        ShootBullets(stateMachine, entity);
                    }
                }
                // 如果没有目标，结束开火。
                else
                {
                    EndFiringBullets(stateMachine, entity);
                }
            }
            /// <summary>
            /// 停止发射子弹。
            /// </summary>
            private void EndFiringBullets(EntityStateMachine stateMachine, Entity boss)
            {
                boss.Target = FindMissileTarget(boss);
                if (boss.Target != null)
                {
                    stateMachine.StartState(boss, STATE_MISSILE);
                }
                else
                {
                    stateMachine.StartState(boss, STATE_IDLE);
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
            private void ShootBullets(EntityStateMachine stateMachine, Entity boss)
            {
                var substateTimer = stateMachine.GetSubStateTimer(boss);
                substateTimer.Run(stateMachine.GetSpeed(boss));
                var intervalCount = substateTimer.PassedIntervalCount(1);
                for (int i = 0; i < intervalCount; i++)
                {
                    ShootABullet(boss);
                }
                if (substateTimer.Expired)
                {
                    EndFiringBullets(stateMachine, boss);
                }
            }
        }
        private class MissileState : EntityStateMachineState
        {
            public MissileState() : base(STATE_MISSILE) { }

            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                var detectTimer = GetDetectTimer(entity);
                substateTimer.ResetTime(30);
                detectTimer.ResetTime(detectIntervalFrames);
                SetMissileDirection(entity, entity.GetFacingDirection());
                entity.PlaySound(VanillaSoundID.gunReload);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var detectTimer = GetDetectTimer(entity);
                detectTimer.Run(stateMachine.GetSpeed(entity));
                if (detectTimer.Expired)
                {
                    detectTimer.ResetTime(detectIntervalFrames);
                    entity.Target = FindMissileTarget(entity);
                }
                if (entity.Target == null || !entity.Target.Exists())
                {
                    stateMachine.StartState(entity, STATE_IDLE);
                    return;
                }

                var substate = stateMachine.GetSubState(entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (!substateTimer.Expired)
                    return;

                switch (substate)
                {
                    case SUBSTATE_MISSILE_AIM:
                        FireMissile(stateMachine, entity);
                        break;
                    case SUBSTATE_MISSILE_FIRED:
                        stateMachine.StartState(entity, STATE_IDLE);
                        break;
                }
            }

            private void FireMissile(EntityStateMachine stateMachine, Entity boss)
            {
                stateMachine.SetSubState(boss, SUBSTATE_MISSILE_FIRED);
                var substateTimer = stateMachine.GetSubStateTimer(boss);
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
        private class JumpState : EntityStateMachineState
        {
            public JumpState() : base(STATE_JUMP) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(24);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (substateTimer.Expired)
                {
                    var substate = stateMachine.GetSubState(entity);
                    switch (substate)
                    {
                        case SUBSTATE_JUMP_READY:
                            stateMachine.SetSubState(entity, SUBSTATE_JUMP_IN_AIR);
                            Jump(entity);
                            break;

                        case SUBSTATE_JUMP_IN_AIR:
                            Vector3 target = GetJumpTarget(entity);
                            var pos = entity.Position;
                            pos.x = pos.x * 0.8f + target.x * 0.2f;
                            pos.z = pos.z * 0.8f + target.z * 0.2f;
                            entity.Position = pos;

                            var spawnParam = entity.GetSpawnParams();
                            spawnParam.SetProperty(EngineEntityProps.FLIP_X, entity.IsFlipX());
                            spawnParam.SetProperty(EngineEntityProps.SCALE, entity.GetScale());
                            spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, entity.GetDisplayScale());
                            entity.Level.Spawn(VanillaEffectID.frankensteinJumpTrail, entity.GetCenter(), entity, spawnParam);
                            if (entity.GetRelativeY() <= 0)
                            {
                                Land(stateMachine, entity);
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

            private void Land(EntityStateMachine stateMachine, Entity boss)
            {
                stateMachine.StartState(boss, STATE_IDLE);

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
        private class PunchState : EntityStateMachineState
        {
            public PunchState() : base(STATE_PUNCH) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.teslaPower);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (!substateTimer.Expired)
                    return;
                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_PUNCH_READY:
                        stateMachine.SetSubState(entity, SUBSTATE_PUNCH_FIRE);
                        substateTimer.ResetTime(3);
                        break;

                    case SUBSTATE_PUNCH_FIRE:
                        stateMachine.SetSubState(entity, SUBSTATE_PUNCH_FINISHED);
                        substateTimer.ResetTime(15);
                        Punch(entity);
                        break;

                    case SUBSTATE_PUNCH_FINISHED:
                        stateMachine.StartState(entity, STATE_IDLE);
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
        private class ShockState : EntityStateMachineState
        {
            public ShockState() : base(STATE_SHOCK) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.PlaySound(VanillaSoundID.teslaPower);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (!substateTimer.Expired)
                    return;
                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_SHOCK_READY:
                        stateMachine.SetSubState(entity, SUBSTATE_SHOCK_FINISHED);
                        entity.Target = FindShockingTarget(entity);
                        if (entity.Target != null)
                        {
                            Shock(entity);
                        }
                        substateTimer.ResetTime(15);
                        break;

                    case SUBSTATE_SHOCK_FINISHED:
                        stateMachine.StartState(entity, STATE_IDLE);
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
                        TNT.Charge(contraption);
                        var arc = level.Spawn(VanillaEffectID.electricArc, boss.Position + outerArmRootOffset + Vector3.left * 100, boss);
                        ElectricArc.Connect(arc, contraption.Position);
                        ElectricArc.UpdateArc(arc);
                    }
                    else if (contraption.IsEntityOf(contrapId))
                    {
                        contraption.ShortCircuit(150);
                        if (!soundPlayed)
                        {
                            contraption.PlaySound(VanillaSoundID.powerOff);
                            soundPlayed = true;
                        }
                        var arc = level.Spawn(VanillaEffectID.electricArc, boss.Position + outerArmRootOffset + Vector3.left * 100, boss);
                        ElectricArc.Connect(arc, contraption.Position);
                        ElectricArc.UpdateArc(arc);
                    }
                }
                boss.PlaySound(VanillaSoundID.teslaAttack);
            }

        }
        #endregion

    }
}

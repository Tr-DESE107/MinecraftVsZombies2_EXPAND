﻿using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    public partial class TheGiant
    {
        #region 状态机
        private class TheGiantStateMachine : EntityStateMachine
        {
            public TheGiantStateMachine()
            {
                AddState(new IdleState());
                AddState(new AppearState());
                AddState(new DisassemblyState());
                AddState(new EyeState());
                AddState(new ArmsState());
                AddState(new RoarState());
                AddState(new BreathState());
                AddState(new PacmanState());
                AddState(new SnakeState());
                AddState(new StunState());
                AddState(new FaintState());
                AddState(new ChaseState());
                AddState(new DeathState());
            }
        }
        #endregion

        public static void Stun(Entity entity, int duration)
        {
            if (entity.IsDead)
                return;
            if (!CanBeStunned(entity))
                return;
            entity.PlaySound(VanillaSoundID.zombieHurt, 0.5f);
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.StartState(entity, STATE_STUNNED);
            var substateTimer = stateMachine.GetSubStateTimer(entity);
            substateTimer.ResetTime(duration);
        }
        public static void SetAppear(Entity entity)
        {
            if (entity.IsDead)
                return;
            entity.Health = 1;
            stateMachine.StartState(entity, STATE_APPEAR);
        }
        private static void SpawnDarkHole(Entity entity)
        {
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * DARK_HOLE_EFFECT_SCALE);
            entity.Spawn(VanillaEffectID.darkHole, entity.Position, param);
            entity.PlaySound(VanillaSoundID.odd);
        }
        private static void RoarLoop(Entity entity)
        {
            entity.Level.ShakeScreen(15, 0, 5);
            foreach (var ent in entity.Level.FindEntities(e => CanRoarStun(entity, e)))
            {
                if (ent.IsEntityOf(VanillaContraptionID.lightningOrb))
                    continue;
                if (ent.IsEntityOf(VanillaContraptionID.noteBlock))
                {
                    if (!ent.HasBuff<NoteBlockChargedBuff>())
                    {
                        ent.AddBuff<NoteBlockChargedBuff>();
                        ent.PlaySound(VanillaSoundID.growBig);
                    }
                    continue;
                }
                ent.Stun(ROAR_STUN_TIME);
            }
        }
        private static bool CanCrawl(Entity entity)
        {
            return entity.GetBounds().min.x > VanillaLevelExt.GetAttackBorderX(false);
        }
        private static void CheckDeath(Entity entity)
        {
            if (!entity.IsDead)
                return;
            if (GetPhase(entity) != PHASE_3)
            {
                stateMachine.StartState(entity, STATE_FAINT);
            }
            else
            {
                stateMachine.StartState(entity, STATE_DEATH);
            }
        }
        private static void ReformToIdle(Entity entity)
        {
            stateMachine.StartState(entity, STATE_IDLE);
            stateMachine.SetSubState(entity, IdleState.SUBSTATE_REFORMED);
            ResetMalleable(entity);
        }

        #region 状态
        private class IdleState : EntityStateMachineState
        {
            public IdleState() : base(STATE_IDLE, ANIMATION_STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(60);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                if (GetPhase(entity) == PHASE_3)
                {
                    if (CanCrawl(entity))
                    {
                        if (IsFlipX(entity))
                        {
                            stateMachine.StartState(entity, STATE_DISASSEMBLY);
                        }
                        else
                        {
                            stateMachine.StartState(entity, STATE_CHASE);
                        }
                    }
                    return;
                }
                else
                {
                    if (entity.IsDead)
                    {
                        stateMachine.StartState(entity, STATE_FAINT);
                        return;
                    }
                }
                if (GetPhase(entity) == PHASE_1 && entity.Health <= entity.GetMaxHealth() * 0.5f)
                {
                    Stun(entity, 30);
                    return;
                }
                UpdateStateSwitch(stateMachine, entity);
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void UpdateStateSwitch(EntityStateMachine stateMachine, Entity entity)
            {
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));
                if (stateTimer.Expired)
                {
                    var substate = stateMachine.GetSubState(entity);
                    if (substate != SUBSTATE_REFORMED)
                    {
                        stateMachine.StartState(entity, STATE_DISASSEMBLY);
                    }
                    else
                    {
                        var nextState = GetNextState(stateMachine, entity);
                        stateMachine.StartState(entity, nextState);
                    }
                }
            }
            private int GetNextState(EntityStateMachine stateMachine, Entity entity)
            {
                var lastState = stateMachine.GetPreviousState(entity);

                var atLeft = AtLeft(entity);
                bool phase2 = GetPhase(entity) == PHASE_2;
                int attackFlag = GetAttackFlag(entity);
                if (atLeft)
                {
                    var attack1Used = (attackFlag & 1) != 0;
                    attackFlag ^= 1;
                    SetAttackFlag(entity, attackFlag);
                    if (attack1Used)
                    {
                        if (!phase2)
                        {
                            lastState = STATE_BREATH;
                            return lastState;
                        }
                        else
                        {
                            lastState = STATE_SNAKE;
                            return lastState;
                        }
                    }
                    lastState = STATE_EYES;
                    var innerTarget = FindEyeBulletTarget(entity, false);
                    var outerTarget = FindEyeBulletTarget(entity, true);
                    if (innerTarget.ExistsAndAlive() || outerTarget.ExistsAndAlive())
                    {
                        return lastState;
                    }
                }
                else
                {
                    var attack1Used = (attackFlag & 2) != 0;
                    attackFlag ^= 2;
                    SetAttackFlag(entity, attackFlag);
                    if (attack1Used)
                    {
                        if (!phase2)
                        {
                            lastState = STATE_ROAR;
                            if (entity.Level.EntityExists(e => CanRoarStun(entity, e)))
                            {
                                return lastState;
                            }
                        }
                        else
                        {
                            lastState = STATE_PACMAN;
                            return lastState;
                        }
                    }
                    lastState = STATE_ARMS;
                    if (CanArmsAttack(entity))
                    {
                        return lastState;
                    }
                }
                return STATE_IDLE;
            }
            public const int SUBSTATE_REFORMED = 1;
        }
        private class AppearState : EntityStateMachineState
        {
            public AppearState() : base(STATE_APPEAR, ANIMATION_STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));
                entity.Health = stateTimer.GetPassedPercentage() * entity.GetMaxHealth();
                if (stateTimer.Expired)
                {
                    SetInactive(entity, true);
                    stateMachine.StartState(entity, STATE_DISASSEMBLY);
                }
            }
        }
        private class DisassemblyState : EntityStateMachineState
        {
            public DisassemblyState() : base(STATE_DISASSEMBLY, ANIMATION_STATE_DISASSEMBLY) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CURL:
                        if (substateTimer.Expired)
                        {
                            SetInactive(entity, true);
                            // 分离。
                            bool atLeft = AtLeft(entity);
                            var targetState = atLeft ? SUBSTATE_TO_RIGHT : SUBSTATE_TO_LEFT;
                            stateMachine.SetSubState(entity, targetState);
                            substateTimer.ResetTime(30);

                            var level = entity.Level;
                            var rng = entity.RNG;
                            var lanes = level.GetMaxLaneCount();
                            var validLanes = Enumerable.Range(0, lanes);
                            var phase2 = GetPhase(entity) == PHASE_2;
                            for (int x = 0; x < ZOMBIE_BLOCK_COLUMNS; x++)
                            {
                                int column = GetZombieBlockStartColumn(entity, x, atLeft);
                                int columnEnd = GetZombieBlockEndColumn(entity, x, atLeft);
                                var lanesPool = validLanes.Shuffle(rng);
                                for (int y = 0; y < lanes; y++)
                                {
                                    var i = y + x * lanes;
                                    var lane = lanesPool.ElementAt(y);

                                    var block = SpawnZombieBlock(entity, entity.Position);
                                    if (!phase2)
                                    {
                                        ZombieBlock.SetMode(block, ZombieBlock.MODE_FLY);
                                    }
                                    else
                                    {
                                        ZombieBlock.SetMode(block, ZombieBlock.MODE_JUMP);
                                        var gravity = 3;
                                        var distance = (rng.Next(2) + 1) * level.GetGridWidth();
                                        block.SetGravity(gravity);
                                        ZombieBlock.SetJumpDistance(block, distance);
                                    }
                                    ZombieBlock.SetStartGrid(block, column, lane);
                                    ZombieBlock.SetTargetGrid(block, columnEnd, lane);
                                    ZombieBlock.SetMoveCooldown(block, 30 + i * ZOMBIE_BLOCK_MOVE_INTERVAL);
                                }
                            }
                            SpawnDarkHole(entity);
                            entity.Position = GetCombinePosition(entity, !atLeft);
                        }
                        break;
                    case SUBSTATE_TO_LEFT:
                    case SUBSTATE_TO_RIGHT:
                        if (substateTimer.Expired)
                        {
                            var blocks = GetZombieBlocks(entity);
                            if (!AreAllZombieBlocksReached(entity))
                            {
                                substateTimer.Reset();
                                break;
                            }
                            stateMachine.SetSubState(entity, SUBSTATE_COMBINE);
                            substateTimer.ResetTime(15);
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                                    ZombieBlock.SetTargetPosition(block, entity.Position);
                                }
                            }
                        }
                        break;
                    case SUBSTATE_COMBINE:
                        if (substateTimer.Expired)
                        {
                            RemoveAllZombieBlocks(entity);
                            SpawnDarkHole(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, AtLeft(entity));
                            ResetMalleable(entity);

                            if (entity.IsDead)
                            {
                                stateMachine.StartState(entity, STATE_FAINT);
                            }
                            else
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_RESTORE);
                                stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_RESTORE);
                                substateTimer.ResetTime(30);
                            }
                        }
                        break;
                    case SUBSTATE_RESTORE:
                        if (substateTimer.Expired)
                        {
                            if (GetPhase(entity) == PHASE_3)
                            {
                                if (IsFlipX(entity))
                                {
                                    stateMachine.StartState(entity, STATE_DISASSEMBLY);
                                }
                                else
                                {
                                    stateMachine.StartState(entity, STATE_CHASE);
                                }
                            }
                            else
                            {
                                ReformToIdle(entity);
                            }
                        }
                        break;
                }
            }
            public const int SUBSTATE_CURL = 0;
            public const int SUBSTATE_TO_LEFT = 1;
            public const int SUBSTATE_TO_RIGHT = 2;
            public const int SUBSTATE_COMBINE = 3;
            public const int SUBSTATE_RESTORE = 4;
            public const int ANIMATION_SUBSTATE_RESTORE = 1;
        }
        private class EyeState : EntityStateMachineState
        {
            public EyeState() : base(STATE_EYES, ANIMATION_STATE_EYES) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_OPEN:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_FIRE);
                            substateTimer.ResetTime(EYE_BULLET_INTERVAL * EYE_BULLET_COUNT);
                        }
                        break;
                    case SUBSTATE_FIRE:
                        for (int i = 0; i < substateTimer.PassedIntervalCount(EYE_BULLET_INTERVAL); i++)
                        {
                            var outerEye = (substateTimer.Frame / EYE_BULLET_INTERVAL) % 2 == 0;
                            var target = FindEyeBulletTarget(entity, outerEye);
                            if (target.ExistsAndAlive())
                            {
                                ShootBullet(entity, target, outerEye);
                                continue;
                            }
                            outerEye = !outerEye;
                            target = FindEyeBulletTarget(entity, outerEye);
                            if (target.ExistsAndAlive())
                            {
                                ShootBullet(entity, target, outerEye);
                                continue;
                            }
                            substateTimer.Frame = 0;
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CLOSE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_CLOSE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_CLOSE:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void ShootBullet(Entity entity, Entity target, bool outerEye)
            {
                entity.Level.ShakeScreen(5, 0, 5);
                var param = entity.GetShootParams();
                var offset = outerEye ? OUTER_EYE_BULLET_OFFSET : INNER_EYE_BULLET_OFFSET;
                offset = entity.ModifyShotOffset(offset);
                param.position = entity.Position + offset;
                param.soundID = null;
                param.damage = entity.GetDamage() * EYE_BULLET_DAMAGE_MULTIPLIER;
                param.projectileID = VanillaProjectileID.reflectionBullet;
                param.velocity = (target.GetCenter() - param.position).normalized * EYE_BULLET_SPEED;
                var spawnParam = entity.GetSpawnParams();
                spawnParam.SetProperty(EngineEntityProps.SCALE, Vector3.one * 2);
                spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * 2);
                param.spawnParam = spawnParam;
                var bullet = entity.ShootProjectile(param);
                bullet.PlaySound(VanillaSoundID.reflection, 0.5f);
                bullet.PlaySound(VanillaSoundID.mineExplode, 0.5f);
            }
            public const int SUBSTATE_OPEN = 0;
            public const int SUBSTATE_FIRE = 1;
            public const int SUBSTATE_CLOSE = 2;
            public const int ANIMATION_SUBSTATE_CLOSE = 1;
        }
        private class ArmsState : EntityStateMachineState
        {
            public ArmsState() : base(STATE_ARMS, ANIMATION_STATE_ARMS) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_OUTER_LIFT:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_OUTER_SMASH);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_OUTER_SMASH);
                            substateTimer.ResetTime(5);
                        }
                        break;
                    case SUBSTATE_OUTER_SMASH:
                        if (substateTimer.Expired)
                        {
                            Smash(entity, true);
                            stateMachine.SetSubState(entity, SUBSTATE_OUTER_SMASHED);
                            substateTimer.ResetTime(25);
                        }
                        break;
                    case SUBSTATE_OUTER_SMASHED:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_LIFT);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_INNER_LIFT);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_INNER_LIFT:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_SMASH);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_INNER_SMASH);
                            substateTimer.ResetTime(5);
                        }
                        break;
                    case SUBSTATE_INNER_SMASH:
                        if (substateTimer.Expired)
                        {
                            Smash(entity, false);
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_SMASHED);
                            substateTimer.ResetTime(25);
                        }
                        break;
                    case SUBSTATE_INNER_SMASHED:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void ShootBullet(Entity entity, Entity target, bool outerEye)
            {
                entity.Level.ShakeScreen(5, 0, 5);
                var param = entity.GetShootParams();
                var offset = outerEye ? OUTER_EYE_BULLET_OFFSET : INNER_EYE_BULLET_OFFSET;
                offset = entity.ModifyShotOffset(offset);
                param.position = entity.Position + offset;
                param.soundID = null;
                param.damage = entity.GetDamage() * EYE_BULLET_DAMAGE_MULTIPLIER;
                param.projectileID = VanillaProjectileID.reflectionBullet;
                param.velocity = (target.GetCenter() - param.position).normalized * EYE_BULLET_SPEED;
                var spawnParam = entity.GetSpawnParams();
                spawnParam.SetProperty(EngineEntityProps.SCALE, Vector3.one * 2);
                spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * 2);
                param.spawnParam = spawnParam;
                var bullet = entity.ShootProjectile(param);
                bullet.PlaySound(VanillaSoundID.reflection, 0.5f);
                bullet.PlaySound(VanillaSoundID.mineExplode, 0.5f);
            }
            public const int SUBSTATE_OUTER_LIFT = 0;
            public const int SUBSTATE_OUTER_SMASH = 1;
            public const int SUBSTATE_OUTER_SMASHED = 2;
            public const int SUBSTATE_INNER_LIFT = 3;
            public const int SUBSTATE_INNER_SMASH = 4;
            public const int SUBSTATE_INNER_SMASHED = 5;
            public const int ANIMATION_SUBSTATE_OUTER_LIFT = 0;
            public const int ANIMATION_SUBSTATE_OUTER_SMASH = 1;
            public const int ANIMATION_SUBSTATE_INNER_LIFT = 2;
            public const int ANIMATION_SUBSTATE_INNER_SMASH = 3;
        }
        private class RoarState : EntityStateMachineState
        {
            public RoarState() : base(STATE_ROAR, ANIMATION_STATE_ROAR) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(15);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_LOOP);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_LOOP);
                            substateTimer.ResetTime(90);
                            entity.PlaySound(VanillaSoundID.giantRoar);
                        }
                        break;
                    case SUBSTATE_LOOP:
                        RoarLoop(entity);
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_END);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_END);
                            substateTimer.ResetTime(15);
                        }
                        break;
                    case SUBSTATE_END:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_START = 0;
            public const int SUBSTATE_LOOP = 1;
            public const int SUBSTATE_END = 2;
            public const int ANIMATION_SUBSTATE_START = 0;
            public const int ANIMATION_SUBSTATE_LOOP = 1;
            public const int ANIMATION_SUBSTATE_END = 2;
        }
        private class BreathState : EntityStateMachineState
        {
            public BreathState() : base(STATE_BREATH, ANIMATION_STATE_BREATH) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_BREATH);
                            substateTimer.ResetTime(45);
                            entity.PlaySound(VanillaSoundID.poisonCast);
                            for (int lane = 0; lane < entity.Level.GetMaxLaneCount(); lane++)
                            {
                                var x = entity.Position.x + 80 * entity.GetFacingX();
                                var z = entity.Level.GetEntityLaneZ(lane);
                                var y = entity.Level.GetGroundY(x, z);
                                var param = entity.GetSpawnParams();
                                var gas = entity.Spawn(VanillaEffectID.mummyGas, new Vector3(x, y, z), param);
                                gas.Velocity = entity.GetFacingDirection() * 1;
                            }
                        }
                        break;
                    case SUBSTATE_BREATH:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_END);
                            substateTimer.ResetTime(15);
                        }
                        break;
                    case SUBSTATE_END:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_START = 0;
            public const int SUBSTATE_BREATH = 1;
            public const int SUBSTATE_END = 2;
        }
        private class PacmanState : EntityStateMachineState
        {
            public PacmanState() : base(STATE_PACMAN, ANIMATION_STATE_PACMAN) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CURL:
                        if (substateTimer.Expired)
                        {
                            SetInactive(entity, true);
                            // 分离。
                            bool atLeft = false;
                            var targetState = SUBSTATE_FORM;
                            stateMachine.SetSubState(entity, targetState);
                            substateTimer.ResetTime(30);

                            var level = entity.Level;
                            var rng = entity.RNG;
                            var lanes = level.GetMaxLaneCount();
                            var validLanes = Enumerable.Range(0, lanes);
                            for (int i = 0; i < PACMAN_BLOCK_COUNT; i++)
                            {
                                var startPosition = GetZombieBlockPosition(entity, i, atLeft);
                                var targetPosition = GetPacmanBlockPosition(entity, i, atLeft);
                                var block = SpawnZombieBlock(entity, entity.Position);
                                ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                                ZombieBlock.SetStartPosition(block, startPosition);
                                ZombieBlock.SetTargetPosition(block, targetPosition);
                                ZombieBlock.SetMoveCooldown(block, 30);
                            }
                            SpawnDarkHole(entity);
                            entity.Position = GetCombinePosition(entity, atLeft);
                        }
                        break;
                    case SUBSTATE_FORM:
                        if (substateTimer.Expired)
                        {
                            if (!AreAllZombieBlocksReached(entity))
                            {
                                substateTimer.Reset();
                                break;
                            }
                            RemoveAllZombieBlocks(entity);

                            bool atLeft = false;
                            SpawnDarkHole(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, atLeft);
                            ResetMalleable(entity);

                            stateMachine.SetSubState(entity, SUBSTATE_PACMAN);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_PACMAN);
                            substateTimer.ResetTime(PACMAN_DURATION);
                            entity.PlaySound(VanillaSoundID.pacmanStart, 0.75f);
                            entity.AddBuff<TheGiantPacmanBuff>();
                            FindPacmanTarget(entity);
                        }
                        break;
                    case SUBSTATE_PACMAN:
                        UpdatePacman(entity);
                        if (!IsPacmanPanic(entity) && (substateTimer.Expired || entity.IsDead))
                        {
                            EndPacman(entity);
                        }
                        break;
                    case SUBSTATE_PACMAN_DEATH:
                        entity.SetAnimationInt("PacmanRotation", 4);
                        entity.SetAnimationInt("PacmanState", 2);
                        if (substateTimer.Expired)
                        {
                            EndPacman(entity);
                        }
                        break;
                    case SUBSTATE_PACMAN_END:
                        if (substateTimer.Expired)
                        {
                            if (!AreAllZombieBlocksReached(entity))
                            {
                                substateTimer.Reset();
                                break;
                            }
                            stateMachine.SetSubState(entity, SUBSTATE_COMBINE);
                            substateTimer.ResetTime(15);
                            var blocks = GetZombieBlocks(entity);
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                                    ZombieBlock.SetTargetPosition(block, entity.Position);
                                }
                            }
                        }
                        break;
                    case SUBSTATE_COMBINE:
                        if (substateTimer.Expired)
                        {
                            bool atLeft = true;
                            RemoveAllZombieBlocks(entity);
                            SpawnDarkHole(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, atLeft);
                            ResetMalleable(entity);
                            if (entity.IsDead)
                            {
                                stateMachine.StartState(entity, STATE_FAINT);
                            }
                            else
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_REFORMED);
                                stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_REFROMED);
                                substateTimer.ResetTime(30);
                            }
                        }
                        break;
                    case SUBSTATE_REFORMED:
                        if (substateTimer.Expired)
                        {
                            ReformToIdle(entity);
                        }
                        break;
                }
            }
            public void UpdatePacman(Entity entity)
            {
                var level = entity.Level;
                var targetGridIndex = GetTargetGridIndex(entity);
                var targetGridPosition = level.GetEntityGridPositionByIndex(targetGridIndex);
                var targetGridDistance = targetGridPosition - entity.Position;
                var reached = entity.MoveOrthogonally(targetGridIndex, PACMAN_MOVE_SPEED);
                if (reached)
                {
                    FindPacmanTarget(entity);
                }

                entity.SetAnimationInt("PacmanRotation", GetPacmanRotation(targetGridDistance));
                entity.SetAnimationInt("PacmanState", IsPacmanPanic(entity) ? 1 : 0);
            }
            public static void EndPacman(Entity entity)
            {
                for (int i = 0; i < PACMAN_BLOCK_COUNT; i++)
                {
                    var targetPosition = GetZombieBlockPosition(entity, i, true);
                    var block = SpawnZombieBlock(entity, entity.Position);
                    ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                    ZombieBlock.SetStartPosition(block, entity.Position);
                    ZombieBlock.SetTargetPosition(block, targetPosition);
                }
                SpawnDarkHole(entity);
                SetInactive(entity, true);
                entity.RemoveBuffs<TheGiantPacmanBuff>();
                entity.RemoveBuffs<TheGiantPacmanKilledBuff>();
                stateMachine.SetSubState(entity, SUBSTATE_PACMAN_END);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.Position = GetCombinePosition(entity, true);
            }
            public static int GetPacmanRotation(Vector3 direction)
            {
                var dir = direction.normalized;
                if (Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
                {
                    if (direction.z < 0)
                    {
                        return 3; // Down.
                    }
                    return 1; // Up.
                }
                if (direction.x > 0)
                {
                    return 2; // Right.
                }
                return 0; // Left.
            }
            public static void FindPacmanTarget(Entity entity)
            {
                var devourer = GetPacmanPanicDevourer(entity);
                if (devourer != null)
                {
                    entity.Target = devourer;
                    var grid = entity.GetEvadeTargetGrid(devourer);
                    SetTargetGridIndex(entity, grid.GetIndex());
                }
                else
                {
                    var level = entity.Level;
                    var target = pacmanDetector.DetectEntityWithTheLeast(entity, e => Vector3.SqrMagnitude(e.Position - entity.Position));
                    entity.Target = target;
                    var grid = entity.GetChaseTargetGrid(entity.Target);
                    SetTargetGridIndex(entity, grid.GetIndex());
                }

            }
            public const int SUBSTATE_CURL = 0;
            public const int SUBSTATE_FORM = 1;
            public const int SUBSTATE_PACMAN = 2;
            public const int SUBSTATE_PACMAN_END = 3;
            public const int SUBSTATE_COMBINE = 4;
            public const int SUBSTATE_REFORMED = 5;
            public const int SUBSTATE_PACMAN_DEATH = 6;
            public const int ANIMATION_SUBSTATE_CURL = 0;
            public const int ANIMATION_SUBSTATE_PACMAN = 1;
            public const int ANIMATION_SUBSTATE_REFROMED = 2;
        }
        private class SnakeState : EntityStateMachineState
        {
            public SnakeState() : base(STATE_SNAKE, ANIMATION_STATE_SNAKE) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CURL:
                        if (substateTimer.Expired)
                        {
                            SetInactive(entity, true);
                            // 分离。
                            bool atLeft = AtLeft(entity);
                            var targetState = SUBSTATE_FORM;
                            stateMachine.SetSubState(entity, targetState);
                            substateTimer.ResetTime(30);

                            var level = entity.Level;
                            var rng = entity.RNG;
                            var lanes = level.GetMaxLaneCount();
                            var validLanes = Enumerable.Range(0, lanes);
                            for (int i = 0; i < SNAKE_BLOCK_COUNT; i++)
                            {
                                var startPosition = GetZombieBlockPosition(entity, i, true);
                                var targetPosition = GetSnakeBlockPosition(entity, i, true);
                                var block = SpawnZombieBlock(entity, entity.Position);
                                ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                                ZombieBlock.SetStartPosition(block, startPosition);
                                ZombieBlock.SetTargetPosition(block, targetPosition);
                                ZombieBlock.SetMoveCooldown(block, 30);
                            }
                            SpawnDarkHole(entity);
                            entity.Position = GetCombinePosition(entity, !atLeft);
                        }
                        break;
                    case SUBSTATE_FORM:
                        if (substateTimer.Expired)
                        {
                            if (!AreAllZombieBlocksReached(entity))
                            {
                                substateTimer.Reset();
                                break;
                            }
                            var blocks = GetZombieBlocks(entity);
                            if (blocks != null)
                            {
                                Entity lastSnakeTail = null;
                                for (int i = 0; i < blocks.Count; i++)
                                {
                                    var blockID = blocks[i];
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    if (i == 0)
                                    {
                                        entity.Position = block.Position;
                                        lastSnakeTail = entity;
                                    }
                                    else if (i < SNAKE_COMBINE_ZOMBIE_BLOCK_COUNT)
                                    {
                                        lastSnakeTail = SpawnSnakeTail(entity, block.Position, lastSnakeTail);
                                    }
                                }
                            }
                            RemoveAllZombieBlocks(entity);

                            SpawnDarkHole(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, false);
                            ResetMalleable(entity);

                            stateMachine.SetSubState(entity, SUBSTATE_SNAKE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_SNAKE);
                            entity.AddBuff<TheGiantSnakeBuff>();

                            SetTargetGridIndex(entity, entity.GetGridIndex());
                            FindSnakeTarget(entity);
                        }
                        break;
                    case SUBSTATE_SNAKE:
                        if (IsSnakeFull(entity) || entity.IsDead)
                        {
                            EndSnake(entity);
                            break;
                        }
                        UpdateSnake(entity);
                        break;
                    case SUBSTATE_SNAKE_DEATH:
                        entity.SetAnimationInt("SnakeState", 1);
                        if (substateTimer.Expired)
                        {
                            EndSnake(entity);
                        }
                        break;
                    case SUBSTATE_SNAKE_END:
                        if (substateTimer.Expired)
                        {
                            if (!AreAllZombieBlocksReached(entity))
                            {
                                substateTimer.Reset();
                                break;
                            }
                            stateMachine.SetSubState(entity, SUBSTATE_COMBINE);
                            substateTimer.ResetTime(15);
                            var blocks = GetZombieBlocks(entity);
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                                    ZombieBlock.SetTargetPosition(block, entity.Position);
                                }
                            }
                        }
                        break;
                    case SUBSTATE_COMBINE:
                        if (substateTimer.Expired)
                        {
                            RemoveAllZombieBlocks(entity);
                            SpawnDarkHole(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, false);
                            ResetMalleable(entity);

                            if (entity.IsDead)
                            {
                                stateMachine.StartState(entity, STATE_FAINT);
                            }
                            else
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_REFORMED);
                                stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_REFROMED);
                                substateTimer.ResetTime(30);
                            }
                        }
                        break;
                    case SUBSTATE_REFORMED:
                        if (substateTimer.Expired)
                        {
                            ReformToIdle(entity);
                        }
                        break;
                }
            }
            public void UpdateSnake(Entity entity)
            {
                TheGiantSnakeTail.MoveTail(entity, SNAKE_MOVE_SPEED);
                var level = entity.Level;
                var targetGridIndex = GetTargetGridIndex(entity);
                var targetGridPosition = level.GetEntityGridPositionByIndex(targetGridIndex);
                var targetGridDistance = targetGridPosition - entity.Position;
                var reached = entity.MoveOrthogonally(targetGridIndex, SNAKE_MOVE_SPEED);
                if (reached)
                {
                    OnSnakeMovedToTarget(entity);
                }
                entity.SetAnimationInt("SnakeRotation", GetSnakeRotation(targetGridDistance));
                entity.SetAnimationInt("SnakeState", 0);
            }
            public void EndSnake(Entity entity)
            {
                RemoveAllZombieBlocks(entity);
                RemoveAllSnakeTails(entity);

                var atLeft = false;
                for (int i = 0; i < SNAKE_BLOCK_COUNT; i++)
                {
                    var targetPosition = GetZombieBlockPosition(entity, i, atLeft);
                    var block = SpawnZombieBlock(entity, entity.Position);
                    ZombieBlock.SetMode(block, ZombieBlock.MODE_TRANSFORM);
                    ZombieBlock.SetStartPosition(block, entity.Position);
                    ZombieBlock.SetTargetPosition(block, targetPosition);
                }

                SpawnDarkHole(entity);
                SetInactive(entity, true);
                entity.RemoveBuffs<TheGiantSnakeBuff>();
                stateMachine.SetSubState(entity, SUBSTATE_SNAKE_END);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
                entity.Position = GetCombinePosition(entity, atLeft);
            }
            public static int GetSnakeRotation(Vector3 direction)
            {
                var dir = direction.normalized;
                if (Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
                {
                    if (direction.z < 0)
                    {
                        return 3; // Down.
                    }
                    return 1; // Up.
                }
                if (direction.x > 0)
                {
                    return 2; // Right.
                }
                return 0; // Left.
            }
            public static void OnSnakeMovedToTarget(Entity entity)
            {
                FindSnakeTarget(entity);
            }
            public static void FindSnakeTarget(Entity entity)
            {
                var level = entity.Level;

                // Eat Food.
                var gridPosition = entity.GetGridPosition();
                var blocks = GetGridZombieBlocks(level, gridPosition);
                foreach (var block in blocks)
                {
                    if (block == null || !block.Exists())
                        continue;
                    block.Remove();
                    entity.PlaySound(VanillaSoundID.pacmanKill);
                    var tail = TheGiantSnakeTail.FindTail(entity);
                    if (tail != null)
                    {
                        SpawnSnakeTail(entity, tail.Position, tail);
                    }
                }

                // Find Next Target.
                var target = level.FindFirstEntityWithTheLeast(e => e.IsEntityOf(VanillaEffectID.zombieBlock), e => Vector3.SqrMagnitude(e.Position - entity.Position));
                if (target == null)
                {
                    target = SpawnZombieBlockAtRandomGrid(entity);
                }

                entity.Target = target;
                var grid = entity.GetChaseTargetGrid(entity.Target, GridValidator);

                var gridIndex = GetTargetGridIndex(entity);
                TheGiantSnakeTail.PassTargetGrids(entity, gridIndex);
                SetTargetGridIndex(entity, grid.GetIndex());


            }
            public static Entity SpawnZombieBlockAtRandomGrid(Entity entity)
            {
                var level = entity.Level;
                var grids = level.GetAllGrids();
                var validGrids = grids.Where(g => !GridHasTail(g));
                if (validGrids.Count() <= 0)
                    return null;
                var grid = validGrids.Random(entity.RNG);
                var pos = grid.GetEntityPosition();
                var spawnPos = pos;
                spawnPos.y = 800;
                var block = SpawnZombieBlock(entity, spawnPos);
                ZombieBlock.SetMode(block, ZombieBlock.MODE_SNAKE_FOOD);
                ZombieBlock.SetStartPosition(block, pos);
                return block;
            }
            public static bool GridHasTail(LawnGrid grid)
            {
                var level = grid.Level;
                return level.EntityExists(e => e.IsEntityOf(VanillaBossID.theGiantSnakeTail) && e.GetGrid() == grid);
            }
            public static bool GridHasTail(LevelEngine level, Vector2Int position)
            {
                return level.EntityExists(e => e.IsEntityOf(VanillaBossID.theGiantSnakeTail) && e.GetColumn() == position.x && e.GetLane() == position.y);
            }
            public static Entity[] GetGridZombieBlocks(LevelEngine level, Vector2Int position)
            {
                return level.FindEntities(e => e.IsEntityOf(VanillaEffectID.zombieBlock) && ZombieBlock.GetMode(e) == ZombieBlock.MODE_SNAKE_FOOD && e.GetColumn() == position.x && e.GetLane() == position.y);
            }
            public static bool IsSnakeFull(Entity head)
            {
                var tails = GetSnakeTails(head);
                return tails != null && tails.Count >= SNAKE_MAX_EAT_COUNT - 1;
            }
            public static bool GridValidator(Entity entity, Vector2Int position)
            {
                if (!entity.Level.ValidateGridOutOfBounds(position))
                    return false;
                if (GridHasTail(entity.Level, position))
                    return false;
                return true;
            }
            public const int SUBSTATE_CURL = 0;
            public const int SUBSTATE_FORM = 1;
            public const int SUBSTATE_SNAKE = 2;
            public const int SUBSTATE_SNAKE_END = 3;
            public const int SUBSTATE_COMBINE = 4;
            public const int SUBSTATE_REFORMED = 5;
            public const int SUBSTATE_SNAKE_DEATH = 6;
            public const int ANIMATION_SUBSTATE_CURL = 0;
            public const int ANIMATION_SUBSTATE_SNAKE = 1;
            public const int ANIMATION_SUBSTATE_REFROMED = 2;
        }
        private class StunState : EntityStateMachineState
        {
            public StunState() : base(STATE_STUNNED, ANIMATION_STATE_STUNNED) { }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_STUNNED:
                        if (substateTimer.PassedFrame(substateTimer.MaxFrame - 18))
                        {
                            entity.Level.ShakeScreen(5, 0, 10);
                            entity.PlaySound(VanillaSoundID.thump);
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_RESTORE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_RESTORE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_RESTORE:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                            if (GetPhase(entity) == PHASE_1 && entity.Health <= entity.GetMaxHealth() * 0.5f)
                            {
                                SetPhase(entity, PHASE_2);
                            }
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_STUNNED = 0;
            public const int SUBSTATE_RESTORE = 1;
            public const int ANIMATION_SUBSTATE_RESTORE = 1;
        }
        private class FaintState : EntityStateMachineState
        {
            public FaintState() : base(STATE_FAINT, ANIMATION_STATE_FAINT) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(90);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_FAINT:
                        if (substateTimer.PassedFrame(substateTimer.MaxFrame - 18))
                        {
                            entity.Level.ShakeScreen(5, 0, 10);
                            entity.PlaySound(VanillaSoundID.thump);
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_HEAL);
                            substateTimer.ResetTime(30);
                            entity.AddBuff<TheGiantPhase3Buff>();
                        }
                        break;
                    case SUBSTATE_HEAL:
                        if (entity.IsDead)
                        {
                            entity.Revive();
                        }
                        SetPhase(entity, PHASE_3);
                        entity.Health = entity.GetMaxHealth() * substateTimer.GetPassedPercentage();
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_ROAR);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_ROAR);
                            substateTimer.ResetTime(90);
                            entity.PlaySound(VanillaSoundID.giantRoar);
                        }
                        break;
                    case SUBSTATE_ROAR:
                        RoarLoop(entity);
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_ROAR_END);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_ROAR_END);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_ROAR_END:
                        if (substateTimer.Expired)
                        {
                            if (IsFlipX(entity))
                            {
                                stateMachine.StartState(entity, STATE_DISASSEMBLY);
                            }
                            else
                            {
                                stateMachine.StartState(entity, STATE_CHASE);
                            }
                        }
                        break;
                }
            }
            public const int SUBSTATE_FAINT = 0;
            public const int SUBSTATE_HEAL = 1;
            public const int SUBSTATE_ROAR = 2;
            public const int SUBSTATE_ROAR_END = 3;
            public const int ANIMATION_SUBSTATE_ROAR = 1;
            public const int ANIMATION_SUBSTATE_ROAR_END = 2;
        }
        private class ChaseState : EntityStateMachineState
        {
            public ChaseState() : base(STATE_CHASE, ANIMATION_STATE_CHASE) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CRAWL_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CRAWL);
                            substateTimer.ResetTime(10);
                            entity.Level.ShakeScreen(5, 0, 10);
                            entity.PlaySound(VanillaSoundID.thump);
                        }
                        break;
                    case SUBSTATE_CRAWL:
                        entity.Velocity = entity.GetFacingDirection() * 1;
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CRAWL_END);
                            substateTimer.ResetTime(10);
                        }
                        break;
                    case SUBSTATE_CRAWL_END:
                        if (substateTimer.Expired)
                        {
                            if (IsFlipX(entity))
                            {
                                stateMachine.StartState(entity, STATE_DISASSEMBLY);
                            }
                            else
                            {
                                if (!CanCrawl(entity))
                                {
                                    stateMachine.StartState(entity, STATE_IDLE);
                                }
                                else
                                {
                                    stateMachine.SetSubState(entity, SUBSTATE_CRAWL_START);
                                    substateTimer.ResetTime(10);
                                }
                            }
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);

                var malleable = GetMalleable(entity);
                malleable = Mathf.Max(0, malleable - MALLEABLE_DECAY_PHASE_3);
                SetMalleable(entity, malleable);

                CheckDeath(entity);
            }
            public const int SUBSTATE_CRAWL_START = 0;
            public const int SUBSTATE_CRAWL = 1;
            public const int SUBSTATE_CRAWL_END = 2;
        }
        private class DeathState : EntityStateMachineState
        {
            public DeathState() : base(STATE_DEATH, ANIMATION_STATE_DEATH) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(120);
                entity.PlaySound(VanillaSoundID.giantRoar);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);

                entity.Level.ShakeScreen(5, 5, 1);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));

                if (stateTimer.Expired)
                {
                    entity.PlaySound(VanillaSoundID.zombieDeath, 0.5f);

                    entity.PlaySound(VanillaSoundID.explosion);

                    var expParam = entity.GetSpawnParams();
                    expParam.SetProperty(EngineEntityProps.SIZE, Vector3.one * 240);
                    var exp = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), expParam);
                    entity.Level.ShakeScreen(20, 0, 30);

                    for (int i = 0; i < 50; i++)
                    {
                        var zombieParam = entity.GetSpawnParams();
                        zombieParam.SetProperty(VanillaEnemyProps.HARMLESS, true);
                        zombieParam.SetProperty(VanillaEnemyProps.NO_REWARD, true);
                        zombieParam.SetProperty(VanillaEntityProps.FALL_RESISTANCE, -10000f);
                        var zombie = entity.Spawn(VanillaEnemyID.zombie, entity.GetCenter(), zombieParam);
                        var xSpeed = zombie.RNG.NextFloat() * 20 - 10;
                        var ySpeed = zombie.RNG.NextFloat() * 10 + 3;
                        var zSpeed = zombie.RNG.NextFloat() * 20 - 10;
                        zombie.Velocity = new Vector3(xSpeed, ySpeed, zSpeed);
                        entity.Remove();
                    }
                }
            }
        }
        #endregion

    }
}

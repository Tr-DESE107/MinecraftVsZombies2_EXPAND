using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
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
                AddState(new DisassemblyState());
                AddState(new StunState());
                AddState(new DeathState());
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
                if (GetPhase(entity) == PHASE_1 && entity.Health <= entity.GetMaxHealth() * 0.5f)
                {
                    SetPhase(entity, PHASE_2);
                    Stun(entity, 30);
                    return;
                }
                UpdateStateSwitch(stateMachine, entity);
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
                        stateMachine.SetPreviousState(entity, nextState);
                    }
                }
            }
            private int GetNextState(EntityStateMachine stateMachine, Entity entity)
            {
                var lastState = stateMachine.GetPreviousState(entity);

                var atLeft = AtLeft(entity);
                if (lastState == STATE_EYES || lastState == STATE_ARMS)
                {
                    bool phase2 = GetPhase(entity) == PHASE_2;
                    if (!phase2)
                    {
                        lastState = atLeft ? STATE_ROAR : STATE_BREATH;
                    }
                    else
                    {
                        lastState = atLeft ? STATE_PACMAN : STATE_SNAKE;
                    }
                }
                else
                {
                    lastState = atLeft ? STATE_EYES : STATE_ARMS;
                }
                return STATE_IDLE;
            }
            public const int SUBSTATE_REFORMED = 1;
        }
        private class DisassemblyState : EntityStateMachineState
        {
            public DisassemblyState() : base(STATE_DISASSEMBLY) { }
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
                            for (int x = 0; x < ZOMBIE_BLOCK_COLUMNS; x++)
                            {
                                int column;
                                int columnEnd;
                                if (atLeft)
                                {
                                    column = GetZombieBlockLeftStartColumn(entity) - x;
                                    columnEnd = GetZombieBlockRightStartColumn(entity) - x + (ZOMBIE_BLOCK_COLUMNS - 1);
                                }
                                else
                                {
                                    column = GetZombieBlockRightStartColumn(entity) + x;
                                    columnEnd = GetZombieBlockLeftStartColumn(entity) + x - (ZOMBIE_BLOCK_COLUMNS - 1);
                                }
                                var lanesPool = validLanes.Shuffle(rng);
                                for (int y = 0; y < lanes; y++)
                                {
                                    var i = y + x * lanes;
                                    var lane = lanesPool.ElementAt(y);
                                    var block = entity.Spawn(VanillaEffectID.zombieBlock, entity.Position, entity.GetSpawnParams());
                                    block.SetParent(entity);
                                    ZombieBlock.SetStartGrid(block, column, lane);
                                    ZombieBlock.SetTargetGrid(block, columnEnd, lane);
                                    ZombieBlock.SetMoveCooldown(block, 30 + i * ZOMBIE_BLOCK_MOVE_INTERVAL);
                                    AddZombieBlock(entity, new EntityID(block));
                                }
                            }
                            var param = entity.GetSpawnParams();
                            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * DARK_HOLE_EFFECT_SCALE);
                            entity.Spawn(VanillaEffectID.darkFlash, entity.Position, param);
                            entity.Position = GetCombinePosition(entity, atLeft);
                            entity.PlaySound(VanillaSoundID.odd);
                        }
                        break;
                    case SUBSTATE_TO_LEFT:
                    case SUBSTATE_TO_RIGHT:
                        if (substateTimer.Expired)
                        {
                            var blocks = GetZombieBlocks(entity);
                            bool allReached = true;
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (!block.ExistsAndAlive())
                                        continue;
                                    if (!ZombieBlock.IsReached(block))
                                    {
                                        substateTimer.Reset();
                                        allReached = false;
                                        break;
                                    }
                                }
                            }
                            if (allReached)
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_COMBINE);
                                substateTimer.ResetTime(15);
                                if (blocks != null)
                                {
                                    foreach (var blockID in blocks)
                                    {
                                        var block = blockID.GetEntity(entity.Level);
                                        if (block == null || !block.Exists())
                                            continue;
                                        ZombieBlock.SetTargetPosition(block, entity.Position);
                                    }
                                }
                            }
                        }
                        break;
                    case SUBSTATE_COMBINE:
                        if (substateTimer.Expired)
                        {
                            var blocks = GetZombieBlocks(entity);
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    block.Remove();
                                }
                            }
                            var param = entity.GetSpawnParams();
                            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * DARK_HOLE_EFFECT_SCALE);
                            entity.Spawn(VanillaEffectID.darkFlash, entity.Position, param);
                            entity.PlaySound(VanillaSoundID.odd);
                            ClearZombieBlocks(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, AtLeft(entity));
                            stateMachine.SetSubState(entity, SUBSTATE_RESTORE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_RESTORE:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                            stateMachine.SetSubState(entity, IdleState.SUBSTATE_REFORMED);
                        }
                        break;
                }
            }
            public const int SUBSTATE_CURL = 0;
            public const int SUBSTATE_TO_LEFT = 1;
            public const int SUBSTATE_TO_RIGHT = 2;
            public const int SUBSTATE_COMBINE = 3;
            public const int SUBSTATE_RESTORE = 4;
        }
        private class StunState : EntityStateMachineState
        {
            public StunState() : base(STATE_STUNNED) { }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));

                if (stateTimer.Expired)
                {
                    stateMachine.StartState(entity, STATE_IDLE);
                }
            }
        }
        private class DeathState : EntityStateMachineState
        {
            public DeathState() : base(STATE_DEATH) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(150);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);

                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));

                if (stateTimer.Expired)
                {
                    entity.PlaySound(VanillaSoundID.witherDeath);
                    entity.PlaySound(VanillaSoundID.explosion);
                    entity.Level.Explode(entity.GetCenter(), 120, entity.GetFaction(), entity.GetDamage() * 18, new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), entity);

                    var param = entity.GetSpawnParams();
                    param.SetProperty(EngineEntityProps.SIZE, Vector3.one * 240);
                    var exp = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), param);
                    entity.Level.ShakeScreen(20, 0, 30);
                    entity.Remove();
                }
            }
        }
        #endregion

    }
}

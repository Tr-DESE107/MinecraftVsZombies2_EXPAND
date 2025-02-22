using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    public partial class Wither
    {
        #region 状态机
        private class WitherStateMachine : EntityStateMachine
        {
            public WitherStateMachine()
            {
                AddState(new AppearState());
                AddState(new IdleState());
                AddState(new ChargeState());
                AddState(new EatState());
                AddState(new SummonState());
                AddState(new DeathState());
            }
        }
        #endregion

        #region 状态
        private class AppearState : EntityStateMachineState
        {
            public AppearState() : base(STATE_APPEAR) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                entity.SetAnimationBool("Shaking", true);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(60);
            }
            public override void OnExit(EntityStateMachine machine, Entity entity)
            {
                base.OnExit(machine, entity);
                entity.SetAnimationBool("Shaking", false);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));
                if (!substateTimer.Expired)
                    return;
                stateMachine.StartState(entity, STATE_IDLE);
            }
        }
        private class IdleState : EntityStateMachineState
        {
            public List<Entity> searchTargetBuffer = new List<Entity>();
            public Detector searchTargetDetector = new WitherDetector();
            public const int MAX_SKULL_CHARGE_MAIN = 90;
            public const int MAX_SKULL_CHARGE = 60;
            public IdleState() : base(STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(300);
            }
            public override void OnExit(EntityStateMachine machine, Entity entity)
            {
                base.OnExit(machine, entity);
                EntityID[] headTargets = GetHeadTargets(entity);
                if (headTargets == null)
                    return;
                for (int i = 0; i < headTargets.Length; i++)
                {
                    headTargets[i] = null;
                }
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                UpdateAction(stateMachine, entity);
                UpdateStateSwitch(stateMachine, entity);
            }
            private void UpdateAction(EntityStateMachine stateMachine, Entity entity)
            {
                var headOpen = GetHeadOpen(entity);
                headOpen *= 0.5f;
                SetHeadOpen(entity, headOpen);

                UpdateArmored(entity);
                UpdateHeads(entity);
            }
            private void UpdateArmored(Entity entity)
            {
                Vector2 targetPos = new Vector2(entity.Position.x, entity.Position.z);
                if (!HasArmor(entity))
                {
                    if (entity.GetRelativeY() < FLY_HEIGHT)
                    {
                        var pos = entity.Position;
                        pos.y += 7;
                        entity.Position = pos;

                        var vel = entity.Velocity;
                        vel.y = Mathf.Max(0, vel.y);
                        entity.Velocity = vel;
                    }
                    //主头负责移动
                    EntityID[] headTargets = GetHeadTargets(entity);
                    if (headTargets != null && headTargets.Length > 0)
                    {
                        var target = headTargets[0]?.GetEntity(entity.Level);
                        if (target.ExistsAndAlive())
                        {
                            var level = entity.Level;
                            var targetColumn = level.GetMaxColumnCount() - 2;
                            targetPos = new Vector2(level.GetEntityColumnX(targetColumn), target.Position.z);
                        }
                    }
                }
                else
                {
                    var level = entity.Level;
                    var targetColumn = level.GetMaxColumnCount() - 2;
                    var targetLane = level.GetMaxLaneCount() / 2;
                    targetPos = new Vector2(level.GetEntityColumnX(targetColumn), level.GetEntityLaneZ(targetLane));
                }
                var thisPos = new Vector2(entity.Position.x, entity.Position.z);
                var dir = targetPos - thisPos;
                if (dir.magnitude > 20)
                {
                    var moveVel = dir.normalized * 20;
                    var pos = entity.Position;
                    pos.x += moveVel.x;
                    pos.z += moveVel.y;
                    entity.Position = pos;
                }
            }
            private void UpdateHeads(Entity entity)
            {
                EntityID[] headTargets = GetHeadTargets(entity);
                if (headTargets == null)
                {
                    headTargets = new EntityID[HEAD_COUNT];
                    SetHeadTargets(entity, headTargets);
                }
                for (var head = 0; head < headTargets.Length; head++)
                {
                    //锁定目标
                    var target = headTargets[head]?.GetEntity(entity.Level);
                    if (!target.ExistsAndAlive())
                    {
                        //搜寻物体，看是否有合格的敌人
                        searchTargetBuffer.Clear();
                        searchTargetDetector.DetectEntities(entity, searchTargetBuffer);
                        //若拥有合格的敌人，则随机选取一个
                        if (searchTargetBuffer.Count > 0)
                        {
                            target = searchTargetBuffer.Random(entity.RNG);
                            headTargets[head] = new EntityID(target);
                        }
                    }
                    else
                    {
                        //判断目标是否合格，否则取消锁定
                        if (!searchTargetDetector.ValidateTarget(entity, target))
                        {
                            target = null;
                            headTargets[head] = null;
                        }
                    }
                    //锁定完毕，开始执行
                    if (target.ExistsAndAlive())
                    {
                        //发射凋灵之首
                        FireSkullUpdate(entity, head, target);
                    }
                }
            }
            private void FireSkullUpdate(Entity entity, int head, Entity target)
            {
                float[] skullCharges = GetSkullCharges(entity);
                if (skullCharges == null)
                {
                    skullCharges = new float[HEAD_COUNT];
                    SetSkullCharges(entity, skullCharges);
                }
                var maxCharges = head == 0 ? MAX_SKULL_CHARGE_MAIN : MAX_SKULL_CHARGE;
                skullCharges[head]++;

                if (skullCharges[head] >= maxCharges)
                {
                    var headPosition = GetHeadPosition(entity, head);
                    var param = entity.GetShootParams();
                    param.position = headPosition;
                    param.projectileID = VanillaProjectileID.witherSkull;
                    param.damage = entity.GetDamage() * 0.75f;
                    param.soundID = VanillaSoundID.witherShoot;
                    param.velocity = 9 * (target.GetCenter() - headPosition).normalized;
                    entity.ShootProjectile(param);
                    skullCharges[head] = 0;
                }
            }
            private void UpdateStateSwitch(EntityStateMachine stateMachine, Entity entity)
            {
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));
                if (!stateTimer.Expired)
                    return;
                var nextState = GetNextState(stateMachine, entity);
                stateMachine.StartState(entity, nextState);
                stateMachine.SetPreviousState(entity, nextState);
            }
            private int GetNextState(EntityStateMachine stateMachine, Entity entity)
            {
                var lastState = stateMachine.GetPreviousState(entity);
                if (lastState == STATE_IDLE || lastState == STATE_EAT)
                {
                    lastState = STATE_CHARGE;
                    if (CanCharge(entity))
                    {
                        return lastState;
                    }
                }

                return STATE_IDLE;
            }
        }
        private class ChargeState : EntityStateMachineState
        {
            public ChargeState() : base(STATE_CHARGE)
            {
            }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(15);

                var lane = FindChargeLane(entity);
                if (lane < 0)
                {
                    lane = entity.GetLane();
                }
                SetChargeTargetLane(entity, lane);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substate = stateMachine.GetSubState(entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var level = entity.Level;
                switch (substate)
                {
                    case SUBSTATE_MOVE:
                        {
                            //移动
                            var column = entity.IsFacingLeft() ? level.GetMaxColumnCount() - 1 : 0;
                            var targetX = level.GetEntityColumnX(column);
                            var targetZ = level.GetEntityLaneZ(GetChargeTargetLane(entity));
                            var targetY = level.GetGroundY(targetX, targetZ);

                            var pos = entity.Position;
                            pos.x = pos.x * 0.85f + targetX * 0.15f;
                            pos.y = pos.y * 0.85f + targetY * 0.15f;
                            pos.z = pos.z * 0.85f + targetZ * 0.15f;
                            entity.Position = pos;

                            if (substateTimer.Expired)
                            {
                                substateTimer.ResetTime(90);
                                stateMachine.SetSubState(entity, SUBSTATE_CHARGING);
                            }
                        }
                        break;

                    case SUBSTATE_CHARGING:
                        //聚能
                        //张嘴
                        var headOpen = GetHeadOpen(entity);
                        headOpen = headOpen * 0.7f + 1 * 0.3f;
                        SetHeadOpen(entity, headOpen);

                        if (substateTimer.Expired)
                        {
                            var vel = entity.Velocity;
                            vel.x = entity.GetFacingX() * -20;
                            entity.Velocity = vel;
                            stateMachine.SetSubState(entity, SUBSTATE_DASH);
                            substateTimer.ResetTime(20);
                        }
                        break;
                    case SUBSTATE_DASH:
                        {
                            var pos = entity.Position;
                            var vel = entity.Velocity;
                            vel.x += entity.GetFacingX() * (substateTimer.MaxFrame - substateTimer.Frame) * 0.5f;

                            bool reachEnd = false;
                            float endX = 0;
                            if (entity.IsFacingLeft())
                            {
                                var column = 0;
                                endX = level.GetEntityColumnX(column);
                                reachEnd = pos.x + vel.x <= endX;
                            }
                            else
                            {
                                var column = level.GetMaxColumnCount() - 1;
                                endX = level.GetEntityColumnX(column);
                                reachEnd = pos.x + vel.x >= endX;
                            }
                            if (reachEnd)
                            {
                                pos.x = endX;

                                vel.x = 0;
                                stateMachine.SetSubState(entity, SUBSTATE_DASH_END);
                                substateTimer.ResetTime(30);
                            }
                            entity.Position = pos;
                            entity.Velocity = vel;
                        }
                        break;
                    case SUBSTATE_DASH_END:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                    case SUBSTATE_INTERRUPTED:
                        if (substateTimer.Expired)
                        {
                            entity.SetAnimationBool("Shaking", false);
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public const int SUBSTATE_MOVE = 0;
            public const int SUBSTATE_CHARGING = 1;
            public const int SUBSTATE_DASH = 2;
            public const int SUBSTATE_DASH_END = 3;
            public const int SUBSTATE_INTERRUPTED = 4;
        }
        private class EatState : EntityStateMachineState
        {
            public EatState() : base(STATE_EAT) { }

            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
            }

            public const int SUBSTATE_READY = 0;
            public const int SUBSTATE_DASH = 1;
            public const int SUBSTATE_EATEN = 2;
        }
        private class SummonState : EntityStateMachineState
        {
            public SummonState() : base(STATE_SUMMON) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
            }
            public override void OnExit(EntityStateMachine machine, Entity entity)
            {
                base.OnExit(machine, entity);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
            }
        }
        private class DeathState : EntityStateMachineState
        {
            public DeathState() : base(STATE_DEATH) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);
            }
        }
        #endregion

    }
}

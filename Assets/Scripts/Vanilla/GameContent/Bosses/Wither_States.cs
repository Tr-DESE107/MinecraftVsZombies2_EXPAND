using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(60);
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
            public const int MAX_SKULL_CHARGE_MAIN = 180;
            public const int MAX_SKULL_CHARGE = 120;
            public IdleState() : base(STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                UpdateAction(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));
                if (!stateTimer.Expired)
                    return;
                var nextState = GetNextState(stateMachine, entity);
                stateMachine.StartState(entity, nextState);
                stateMachine.SetPreviousState(entity, nextState);
            }
            private void RotateHeadUpdate(Entity entity, int head, Entity target)
            {
                var headPosition = GetHeadPosition(entity, head);
                float targetAngle = 0;
                if (target.ExistsAndAlive())
                {
                    Vector3 targetDirection = target.GetCenter() - headPosition;
                    Vector3 facingDirection = entity.GetFacingDirection();
                    var targetDir2D = new Vector2(targetDirection.x, targetDirection.z);
                    var facingDir2D = new Vector2(facingDirection.x, facingDirection.z);
                    targetAngle = Vector2.SignedAngle(targetDir2D, facingDir2D);
                }
                var headAngles = GetHeadAngles(entity);
                if (headAngles == null)
                {
                    headAngles = new float[HEAD_COUNT];
                    SetHeadAngles(entity, headAngles);
                }

                var angle = headAngles[head];
                if (angle > 180)
                {
                    angle = angle - 360;
                }
                if (targetAngle > angle)
				{
                    if (angle + HEAD_ROTATE_SPEED > targetAngle)
                    {
                        angle = targetAngle;
                    }
                    else
                    {
                        angle += HEAD_ROTATE_SPEED;
                    }
                }
                else
                {
                    if (angle - HEAD_ROTATE_SPEED < targetAngle)
                    {
                        angle = targetAngle;
                    }
                    else
                    {
                        angle -= HEAD_ROTATE_SPEED;
                    }
                }
                headAngles[head] = (angle + 360) % 360;
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
                if (dir.magnitude > 10)
                {
                    var moveVel = dir.normalized * 10;
                    var pos = entity.Position;
                    pos.x += moveVel.x;
                    pos.z += moveVel.y;
                    entity.Position = pos;
                }
            }
            private void UpdateAction(EntityStateMachine stateMachine, Entity entity)
            {
				var headOpen = GetHeadOpen(entity);
				headOpen *= 0.5f;
				SetHeadOpen(entity, headOpen);

                UpdateArmored(entity);

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

                    //转头
                    RotateHeadUpdate(entity, head, target);
                    //锁定完毕，开始执行
                    if (target.ExistsAndAlive())
                    {
                        //发射凋灵之首
                        FireSkullUpdate(entity, head, target);
			        }
		        }
            }
            private int GetNextState(EntityStateMachine stateMachine, Entity entity)
            {
                var lastState = stateMachine.GetPreviousState(entity);

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
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                RunTimer(stateMachine, entity);
            }
            private void RunTimer(EntityStateMachine stateMachine, Entity entity)
            {
                var substate = stateMachine.GetSubState(entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                switch (substate)
                {
                    case SUBSTATE_READY:
                        break;
                    case SUBSTATE_CHARGING:
                        break;
                    case SUBSTATE_DASH:
                        break;
                    case SUBSTATE_INTERRUPTED:
                        break;
                }
            }
            public const int SUBSTATE_READY = 0;
            public const int SUBSTATE_CHARGING = 1;
            public const int SUBSTATE_DASH = 2;
            public const int SUBSTATE_INTERRUPTED = 3;
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

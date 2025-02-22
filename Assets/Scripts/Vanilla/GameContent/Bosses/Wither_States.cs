using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
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

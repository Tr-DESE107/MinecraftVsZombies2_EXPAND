using System.Collections.Generic;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.Vanilla.Entities
{
    public class EntityStateMachine
    {
        public EntityStateMachine()
        {

        }
        public void Init(Entity entity)
        {
            EnterState(entity, entity.State);
        }
        public void UpdateAI(Entity entity)
        {
            var state = GetState(entity.State);
            if (state == null)
                return;
            state.OnUpdateAI(this, entity);
        }
        public void UpdateLogic(Entity entity)
        {
            var state = GetState(entity.State);
            if (state == null)
                return;
            state.OnUpdateLogic(this, entity);
        }
        public void AddState(EntityStateMachineState state)
        {
            states.Add(state);
        }
        public EntityStateMachineState GetState(int stateNumber)
        {
            foreach (var state in states)
            {
                if (state.state == stateNumber)
                    return state;
            }
            return null;
        }
        public int GetSubState(Entity entity)
        {
            return entity.GetBehaviourField<int>(PROP_SUBSTATE);
        }
        public void SetSubState(Entity entity, int value)
        {
            entity.SetBehaviourField(PROP_SUBSTATE, value);
            entity.SetAnimationInt("SubState", value);
        }
        public virtual float GetSpeed(Entity entity)
        {
            return 1;
        }
        public int GetPreviousState(Entity boss) => boss.GetBehaviourField<int>(PROP_PREVIOUS_STATE);
        public void SetPreviousState(Entity boss, int value) => boss.SetBehaviourField(PROP_PREVIOUS_STATE, value);

        public FrameTimer GetStateTimer(Entity entity)
        {
            var timer = entity.GetBehaviourField<FrameTimer>(PROP_STATE_TIMER);
            if (timer == null)
            {
                timer = new FrameTimer();
                entity.SetBehaviourField(PROP_STATE_TIMER, timer);
            }
            return timer;
        }

        public FrameTimer GetSubStateTimer(Entity entity)
        {
            var timer = entity.GetBehaviourField<FrameTimer>(PROP_SUBSTATE_TIMER);
            if (timer == null)
            {
                timer = new FrameTimer();
                entity.SetBehaviourField(PROP_SUBSTATE_TIMER, timer);
            }
            return timer;
        }
        public void StartState(Entity entity, int state)
        {
            ExitState(entity, entity.State);
            EnterState(entity, state);
        }
        private void EnterState(Entity entity, int stateNum)
        {
            var state = GetState(stateNum);
            if (state == null)
                return;

            SetSubState(entity, 0);
            entity.State = stateNum;

            var substateTimer = GetSubStateTimer(entity);
            var nextStateTimer = GetStateTimer(entity);

            substateTimer.Stop();
            nextStateTimer.Stop();

            entity.SetAnimationInt("State", stateNum);

            state.OnEnter(this, entity);
            OnEnterState(entity, stateNum);
        }
        private void ExitState(Entity entity, int stateNum)
        {
            var state = GetState(stateNum);
            if (state == null)
                return;
            state.OnExit(this, entity);
            OnExitState(entity, stateNum);
        }
        protected virtual void OnEnterState(Entity entity, int state) { }
        protected virtual void OnExitState(Entity entity, int state) { }


        private List<EntityStateMachineState> states = new List<EntityStateMachineState>();

        [EntityPropertyRegistry(PROP_REGION)]
        private static readonly VanillaEntityPropertyMeta PROP_SUBSTATE = new VanillaEntityPropertyMeta("SubState");
        [EntityPropertyRegistry(PROP_REGION)]
        private static readonly VanillaEntityPropertyMeta PROP_PREVIOUS_STATE = new VanillaEntityPropertyMeta("PreviousState");
        [EntityPropertyRegistry(PROP_REGION)]
        private static readonly VanillaEntityPropertyMeta PROP_STATE_TIMER = new VanillaEntityPropertyMeta("StateTimer");
        [EntityPropertyRegistry(PROP_REGION)]
        private static readonly VanillaEntityPropertyMeta PROP_SUBSTATE_TIMER = new VanillaEntityPropertyMeta("SubStateTimer");
        public const string PROP_REGION = "state_machine";
    }
}

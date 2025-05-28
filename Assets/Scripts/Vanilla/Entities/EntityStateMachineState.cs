using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class EntityStateMachineState
    {
        protected EntityStateMachineState(int state) : this(state, 0)
        {
        }
        protected EntityStateMachineState(int state, int animationState)
        {
            this.state = state;
            this.animationState = animationState;
        }
        public int state;
        public int animationState;
        public virtual void OnEnter(EntityStateMachine machine, Entity entity) { }
        public virtual void OnUpdateAI(EntityStateMachine machine, Entity entity) { }
        public virtual void OnUpdateLogic(EntityStateMachine machine, Entity entity) { }
        public virtual void OnExit(EntityStateMachine machine, Entity entity) { }
    }
}

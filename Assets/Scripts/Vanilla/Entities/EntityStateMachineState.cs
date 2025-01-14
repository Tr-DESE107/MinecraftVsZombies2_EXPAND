using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class EntityStateMachineState
    {
        protected EntityStateMachineState(int state)
        {
            this.state = state;
        }
        public int state;
        public virtual void OnEnter(EntityStateMachine machine, Entity entity) { }
        public virtual void OnUpdateAI(EntityStateMachine machine, Entity entity) { }
        public virtual void OnUpdateLogic(EntityStateMachine machine, Entity entity) { }
        public virtual void OnExit(EntityStateMachine machine, Entity entity) { }
    }
}

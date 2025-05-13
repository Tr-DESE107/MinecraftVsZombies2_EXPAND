using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class AIEntityBehaviour : EntityBehaviourDefinition
    {
        public AIEntityBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen())
            {
                UpdateAI(entity);
            }
            UpdateLogic(entity);
        }
        protected virtual void UpdateLogic(Entity entity)
        {
        }
        protected virtual void UpdateAI(Entity entity)
        {
        }
    }
}
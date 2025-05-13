using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class CartBehaviour : EntityBehaviourDefinition
    {
        protected CartBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostTrigger(Entity entity)
        {
        }
    }

}
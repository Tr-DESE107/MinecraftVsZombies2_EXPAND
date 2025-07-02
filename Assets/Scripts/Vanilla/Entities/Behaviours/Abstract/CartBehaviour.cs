using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public interface ICartBehaviour
    {
        void PostTrigger(Entity entity);
        void PostCrush(Entity entity, Entity enemy);
    }
    public abstract class CartBehaviour : EntityBehaviourDefinition, ICartBehaviour
    {
        protected CartBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostTrigger(Entity entity)
        {
        }
        public virtual void PostCrush(Entity cart, Entity enemy)
        {
        }
    }

}
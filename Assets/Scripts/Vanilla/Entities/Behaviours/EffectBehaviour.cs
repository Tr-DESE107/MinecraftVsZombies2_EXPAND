using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class EffectBehaviour : VanillaEntityBehaviour
    {
        protected EffectBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Remove();
                }
            }
        }
    }
}

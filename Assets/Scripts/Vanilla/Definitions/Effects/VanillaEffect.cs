using MVZ2.GameContent;
using PVZEngine.Entities;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEffect : VanillaEntity
    {
        protected VanillaEffect(string nsp, string name) : base(nsp, name)
        {
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
        public override int Type => EntityTypes.EFFECT;
    }
}

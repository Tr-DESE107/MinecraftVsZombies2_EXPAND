using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaBoss : VanillaEntity
    {
        protected VanillaBoss(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.RightFaction);
        }
        public override int Type => EntityTypes.OBSTACLE;
    }
}

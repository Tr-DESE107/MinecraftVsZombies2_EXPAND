using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public abstract class VanillaObstacle : VanillaEntity
    {
        protected VanillaObstacle(string nsp, string name) : base(nsp, name)
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

using PVZEngine.Definitions;
using PVZEngine.LevelManaging;

namespace MVZ2.Vanilla
{
    public abstract class VanillaObstacle : VanillaEntity
    {
        protected VanillaObstacle(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.FRICTION, 0.2f);
            SetProperty(EntityProperties.FALL_DAMAGE, 22.5f);
            SetProperty(EntityProperties.FACE_LEFT_AT_DEFAULT, true);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.RightFaction);
        }
        public override int Type => EntityTypes.OBSTACLE;
    }
}

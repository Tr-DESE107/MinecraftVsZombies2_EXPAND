using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaBoss : VanillaEntity
    {
        protected VanillaBoss(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.FRICTION, 0.15f);
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

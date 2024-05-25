using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaProjectile : VanillaEntity
    {
        protected VanillaProjectile(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ProjectileProperties.MAX_TIMEOUT, 1800);
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            var projectile = entity.ToProjectile();
            if (projectile.WillDestroyOutsideLawn() && IsOutsideView(projectile))
            {
                entity.Remove();
                return;
            }
        }
        public virtual bool IsOutsideView(Projectile proj)
        {
            var bounds = proj.GetBounds();
            var position = proj.Pos;
            return position.x < 180 - bounds.extents.x ||
                position.x > 1100 + bounds.extents.x ||
                position.z > 550 ||
                position.z < -50 ||
                position.y > 1000 ||
                position.y < -1000;
        }
        public override int Type => EntityTypes.PROJECTILE;
    }

}
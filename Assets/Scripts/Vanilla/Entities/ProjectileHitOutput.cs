using PVZEngine.Armors;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public class ProjectileHitOutput
    {
        public Entity Projectile { get; set; }
        public Entity Other { get; set; }
        public Armor Shield { get; set; }
        public IEntityCollider Collider { get; set; }
        public bool Pierce { get; set; }
    }
}

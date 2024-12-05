using PVZEngine.Armors;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public class ProjectileHitInput
    {
        public Entity Projectile { get; set; }
        public Entity Other { get; set; }
        public bool Pierce { get; set; }
    }
}

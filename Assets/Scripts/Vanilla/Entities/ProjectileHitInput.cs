using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Entities
{
    public class ProjectileHitInput : IInterruptSource
    {
        public Entity Projectile { get; set; }
        public Entity Other { get; set; }
        public bool Pierce { get; set; }
        public bool IsInterrupted { get; private set; }
        public void Cancel()
        {
            IsInterrupted = true;
        }
    }
}

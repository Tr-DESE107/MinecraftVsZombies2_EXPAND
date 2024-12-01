using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public class ProjectileHitResult
    {
        public Entity Projectile { get; set; }
        public Entity Other { get; set; }
        public bool Pierce { get; set; }
    }
}

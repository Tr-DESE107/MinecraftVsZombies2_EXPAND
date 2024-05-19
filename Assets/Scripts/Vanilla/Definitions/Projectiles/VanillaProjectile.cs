using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaProjectile : EntityDefinition
    {
        public VanillaProjectile()
        {
            propertyDict.SetProperty(ProjectileProperties.MAX_TIMEOUT, 1800);
        }
        public override int Type => EntityTypes.PROJECTILE;
    }

}
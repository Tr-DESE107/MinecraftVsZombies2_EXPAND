using UnityEngine;

namespace PVZEngine.Entities
{
    public class EntityHitbox : Hitbox
    {
        public EntityHitbox(Entity entity) : base(entity)
        {
        }
        public override Vector3 GetSize() => Entity.Cache.Size;
        public override Vector3 GetPivot() => Entity.Cache.BoundsPivot;
        public override Vector3 GetOffset() => Vector3.zero;
    }
}

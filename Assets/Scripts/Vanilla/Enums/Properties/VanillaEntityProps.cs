using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaEntityProps
    {
        public const string RANGE = "range";
        public const string SHOT_VELOCITY = "shotVelocity";
        public const string SHOT_OFFSET = "shotOffset";
        public const string SHOOT_SOUND = "shootSound";
        public const string PROJECTILE_ID = "projectileId";

        public static float GetRange(this Entity entity)
        {
            return entity.GetProperty<float>(RANGE);
        }
        public static Vector3 GetShotVelocity(this Entity entity)
        {
            return entity.GetProperty<Vector3>(SHOT_VELOCITY);
        }
        public static Vector3 GetShotOffset(this Entity entity)
        {
            return entity.GetProperty<Vector3>(SHOT_OFFSET);
        }
        public static NamespaceID GetShootSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(SHOOT_SOUND);
        }
        public static NamespaceID GetProjectileID(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PROJECTILE_ID);
        }
    }
}

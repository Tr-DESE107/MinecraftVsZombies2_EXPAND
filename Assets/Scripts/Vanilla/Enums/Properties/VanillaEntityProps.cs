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
        public const string IS_LIGHT_SOURCE = "isLightSource";
        public const string LIGHT_RANGE = "lightRange";
        public const string LIGHT_COLOR = "lightColor";

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
        public static bool IsLightSource(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_LIGHT_SOURCE);
        }
        public static Vector2 GetLightRange(this Entity entity)
        {
            return entity.GetProperty<Vector2>(LIGHT_RANGE);
        }
        public static Color GetLightColor(this Entity entity)
        {
            return entity.GetProperty<Color>(LIGHT_COLOR);
        }
    }
}

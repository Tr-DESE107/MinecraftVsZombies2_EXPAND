using PVZEngine;
using PVZEngine.Definitions;
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

        public const string DAMAGE = "damage";
        public const string ATTACK_SPEED = "attackSpeed";
        public const string PRODUCE_SPEED = "produceSpeed";
        public const string ETHEREAL = "ethereal";
        public const string FALL_DAMAGE = "fallDamage";
        public const string CAN_DISABLE = "canDisable";
        public const string INVISIBLE = "invisible";

        public static float GetFallDamage(this Entity entity)
        {
            return entity.GetProperty<float>(FALL_DAMAGE);
        }
        public static void SetFallDamage(this Entity entity, float value)
        {
            entity.SetProperty(FALL_DAMAGE, value);
        }
        public static float GetDamage(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(DAMAGE, ignoreBuffs: ignoreBuffs);
        }
        public static void SetDamage(this Entity entity, float value)
        {
            entity.SetProperty(DAMAGE, value);
        }
        public static bool IsInvisible(this Entity entity)
        {
            return entity.GetProperty<bool>(INVISIBLE);
        }
        public static bool IsEthereal(this Entity entity)
        {
            return entity.GetProperty<bool>(ETHEREAL);
        }
        public static float GetAttackSpeed(this Entity entity)
        {
            return entity.GetProperty<float>(ATTACK_SPEED);
        }
        public static float GetProduceSpeed(this Entity entity)
        {
            return entity.GetProperty<float>(PRODUCE_SPEED);
        }
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

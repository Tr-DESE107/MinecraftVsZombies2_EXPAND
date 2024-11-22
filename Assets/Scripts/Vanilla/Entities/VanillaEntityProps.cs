using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEntityProps
    {
        public const string RANGE = "range";
        public const string SHOT_VELOCITY = "shotVelocity";
        public const string SHOT_OFFSET = "shotOffset";
        public const string SHOOT_SOUND = "shootSound";
        public const string PROJECTILE_ID = "projectileID";

        public const string DAMAGE = "damage";
        public const string ATTACK_SPEED = "attackSpeed";
        public const string PRODUCE_SPEED = "produceSpeed";
        public const string ETHEREAL = "ethereal";
        public const string FALL_DAMAGE = "fallDamage";
        public const string CAN_DISABLE = "canDisable";
        public const string INVISIBLE = "invisible";
        public const string AI_FROZEN = "aiFrozen";

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
        public static bool IsAIFrozen(this Entity entity)
        {
            return entity.GetProperty<bool>(AI_FROZEN);
        }
        public const string DEATH_SOUND = "deathSound";
        public const string PLACE_SOUND = "placeSound";
        public const string MAX_TIMEOUT = "maxTimeout";

        public const string CHANGING_LANE = "isChangingLane";
        public const string CHANGE_LANE_SPEED = "changeLaneSpeed";
        public const string CHANGE_LANE_TARGET = "changeLaneTarget";
        public const string CHANGE_LANE_SOURCE = "changeLaneSource";

        public const string SHADOW_HIDDEN = "shadowHidden";
        public const string SHADOW_ALPHA = "shadowAlpha";
        public const string SHADOW_SCALE = "shadowScale";
        public const string SHADOW_OFFSET = "shadowOffset";

        public const string SORTING_LAYER = "sortingLayer";
        public const string SORTING_ORDER = "sortingOrder";

        public const string UPDATE_BEFORE_GAME = "updateBeforeGame";

        public static bool CanUpdateBeforeGameStart(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_BEFORE_GAME);
        }


        public const string IS_LIGHT_SOURCE = "isLightSource";
        public const string LIGHT_RANGE = "lightRange";
        public const string LIGHT_COLOR = "lightColor";
        public static bool IsLightSource(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_LIGHT_SOURCE);
        }
        public static Vector3 GetLightRange(this Entity entity)
        {
            return entity.GetProperty<Vector3>(LIGHT_RANGE);
        }
        public static Color GetLightColor(this Entity entity)
        {
            return entity.GetProperty<Color>(LIGHT_COLOR);
        }
    }
}

using UnityEngine;

namespace PVZEngine.Entities
{
    public static class EngineEntityProps
    {
        public const string MAX_HEALTH = "maxHealth";

        public const string SHELL = "shell";
        public const string PLACEMENT = "placement";

        public const string SCALE = "scale";
        public const string DISPLAY_SCALE = "displayScale";
        public const string FRICTION = "friction";
        public const string GRAVITY = "gravity";
        public const string VELOCITY_DAMPEN = "velocityDampen";
        public const string SIZE = "size";
        public const string BOUNDS_OFFSET = "boundsOffset";
        public const string GROUND_LIMIT_OFFSET = "groundLimitOffset";

        public const string HEALTH = "health";
        public const string TINT = "tint";
        public const string COLOR_OFFSET = "colorOffset";
        public const string FACE_LEFT_AT_DEFAULT = "faceLeftAtDefault";
        public static bool FaceLeftAtDefault(this Entity entity)
        {
            return entity.GetProperty<bool>(FACE_LEFT_AT_DEFAULT);
        }

        public const string FACTION = "faction";
        public const string INVINCIBLE = "invincible";

        public const string MODEL_ID = "modelId";

        public static bool IsInvincible(this Entity entity)
        {
            return entity.GetProperty<bool>(INVINCIBLE);
        }
        public static float GetGravity(this EntityDefinition entity)
        {
            return entity.GetProperty<float>(GRAVITY);
        }
        public static float GetGravity(this Entity entity)
        {
            return entity.GetProperty<float>(GRAVITY);
        }
        public static void SetGravity(this Entity entity, float value)
        {
            entity.SetProperty(GRAVITY, value);
        }
        public static Vector3 GetScale(this Entity entity)
        {
            return entity.GetProperty<Vector3>(SCALE);
        }
        public static void SetScale(this Entity entity, Vector3 value)
        {
            entity.SetProperty(SCALE, value);
        }
        public static Vector3 GetDisplayScale(this Entity entity)
        {
            return entity.GetProperty<Vector3>(DISPLAY_SCALE);
        }
        public static void SetDisplayScale(this Entity entity, Vector3 value)
        {
            entity.SetProperty(DISPLAY_SCALE, value);
        }
        public static Vector3 GetVelocityDampen(this Entity entity)
        {
            return entity.GetProperty<Vector3>(VELOCITY_DAMPEN);
        }
        public static void SetVelocityDampen(this Entity entity, Vector3 value)
        {
            entity.SetProperty(VELOCITY_DAMPEN, value);
        }
        public static float GetFriction(this Entity entity)
        {
            return entity.GetProperty<float>(FRICTION);
        }
        public static void SetFriction(this Entity entity, float value)
        {
            entity.SetProperty(FRICTION, value);
        }
        public static Color GetTint(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Color>(TINT, ignoreBuffs: ignoreBuffs);
        }
        public static void SetTint(this Entity entity, Color value)
        {
            entity.SetProperty(TINT, value);
        }
        public static Color GetColorOffset(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Color>(COLOR_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public static void SetColorOffset(this Entity entity, Color value)
        {
            entity.SetProperty(COLOR_OFFSET, value);
        }
        public static int GetFaction(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<int>(FACTION, ignoreBuffs: ignoreBuffs);
        }
        public static void SetFaction(this Entity entity, int value)
        {
            entity.SetProperty(FACTION, value);
        }
        public static float GetGroundLimitOffset(this Entity entity)
        {
            return entity.GetProperty<float>(EngineEntityProps.GROUND_LIMIT_OFFSET);
        }
        public static Vector3 GetSize(this EntityDefinition entity)
        {
            return entity.GetProperty<Vector3>(SIZE);
        }
        public static Vector3 GetSize(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Vector3>(SIZE, ignoreBuffs: ignoreBuffs);
        }
        public static void SetSize(this Entity entity, Vector3 value)
        {
            entity.SetProperty(SIZE, value);
        }
        public static Vector3 GetBoundsOffset(this EntityDefinition entity)
        {
            return entity.GetProperty<Vector3>(BOUNDS_OFFSET);
        }
        public static Vector3 GetBoundsOffset(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Vector3>(BOUNDS_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public static void SetBoundsOffset(this Entity entity, Vector3 value)
        {
            entity.SetProperty(BOUNDS_OFFSET, value);
        }
        public static float GetMaxHealth(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
        public static NamespaceID GetShellID(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<NamespaceID>(SHELL, ignoreBuffs: ignoreBuffs);
        }
        public static NamespaceID GetPlacementID(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID>(PLACEMENT);
        }
        public static void SetShellID(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(SHELL, value);
        }
    }
}

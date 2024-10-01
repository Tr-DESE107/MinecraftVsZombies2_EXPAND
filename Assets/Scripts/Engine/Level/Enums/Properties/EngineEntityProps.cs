using UnityEngine;

namespace PVZEngine.Level
{
    public static class EngineEntityProps
    {
        public const string MAX_HEALTH = "maxHealth";

        public const string SHELL = "shell";

        public const string FRICTION = "friction";
        public const string GRAVITY = "gravity";
        public const string SIZE = "size";
        public const string CAN_UNDER_GROUND = "canUnderGround";

        public const string HEALTH = "health";
        public const string TINT = "tint";
        public const string COLOR_OFFSET = "colorOffset";
        public const string FACE_LEFT_AT_DEFAULT = "faceLeftAtDefault";
        public const string FACTION = "faction";
        public const string INVINCIBLE = "invincible";
        public static bool IsInvincible(this Entity entity)
        {
            return entity.GetProperty<bool>(INVINCIBLE);
        }
        public static float GetGravity(this Entity entity)
        {
            return entity.GetProperty<float>(GRAVITY);
        }
        public static void SetGravity(this Entity entity, float value)
        {
            entity.SetProperty(GRAVITY, value);
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
        public static Vector3 GetSize(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Vector3>(SIZE, ignoreBuffs: ignoreBuffs);
        }
        public static void SetSize(this Entity entity, Vector3 value)
        {
            entity.SetProperty(SIZE, value);
        }
        public static float GetMaxHealth(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
        public static NamespaceID GetShellID(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<NamespaceID>(SHELL, ignoreBuffs: ignoreBuffs);
        }
        public static void SetShellID(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(SHELL, value);
        }
    }
}

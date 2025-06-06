using UnityEngine;

namespace PVZEngine.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class EngineEntityProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }

        #region 默认朝左
        public static readonly PropertyMeta<bool> FACE_LEFT_AT_DEFAULT = Get<bool>("faceLeftAtDefault");
        public static bool FaceLeftAtDefault(this Entity entity)
        {
            return entity.GetProperty<bool>(FACE_LEFT_AT_DEFAULT);
        }
        #endregion

        #region 无敌

        public static readonly PropertyMeta<bool> INVINCIBLE = Get<bool>("invincible");
        public static bool IsInvincible(this Entity entity)
        {
            return entity.GetProperty<bool>(INVINCIBLE);
        }
        #endregion

        #region 重力
        public static readonly PropertyMeta<float> GRAVITY = Get<float>("gravity");
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
        #endregion

        #region 水平翻转

        public static readonly PropertyMeta<bool> FLIP_X = Get<bool>("flipX");
        public static bool IsFlipX(this Entity entity)
        {
            return entity.GetProperty<bool>(FLIP_X);
        }
        public static void SetFlipX(this Entity entity, bool flipX)
        {
            entity.SetProperty<bool>(FLIP_X, flipX);
        }
        #endregion


        #region 体积
        public static readonly PropertyMeta<Vector3> SIZE = Get<Vector3>("size");
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
        #endregion

        #region 缩放
        public static readonly PropertyMeta<Vector3> SCALE = Get<Vector3>("scale");
        public static Vector3 GetScale(this Entity entity)
        {
            return entity.GetProperty<Vector3>(SCALE);
        }
        public static void SetScale(this Entity entity, Vector3 value)
        {
            entity.SetProperty(SCALE, value);
        }
        public static Vector3 GetFinalScale(this Entity entity)
        {
            var scale = entity.GetScale();
            var flipX = entity.IsFlipX() ? -1 : 1;
            scale.x *= flipX;
            return scale;
        }
        #endregion

        #region 显示缩放
        public static readonly PropertyMeta<Vector3> DISPLAY_SCALE = Get<Vector3>("displayScale");
        public static Vector3 GetFinalDisplayScale(this Entity entity)
        {
            var scale = entity.GetDisplayScale();
            var flipX = entity.IsFlipX() ? -1 : 1;
            scale.x *= flipX;
            return scale;
        }
        public static Vector3 GetDisplayScale(this Entity entity)
        {
            return entity.GetProperty<Vector3>(DISPLAY_SCALE);
        }
        public static void SetDisplayScale(this Entity entity, Vector3 value)
        {
            entity.SetProperty(DISPLAY_SCALE, value);
        }
        #endregion

        #region 移速阻尼
        public static readonly PropertyMeta<Vector3> VELOCITY_DAMPEN = Get<Vector3>("velocityDampen");
        public static Vector3 GetVelocityDampen(this Entity entity)
        {
            return entity.GetProperty<Vector3>(VELOCITY_DAMPEN);
        }
        public static void SetVelocityDampen(this Entity entity, Vector3 value)
        {
            entity.SetProperty(VELOCITY_DAMPEN, value);
        }
        #endregion

        #region 摩擦力
        public static readonly PropertyMeta<float> FRICTION = Get<float>("friction");
        public static float GetFriction(this Entity entity)
        {
            return entity.GetProperty<float>(FRICTION);
        }
        public static void SetFriction(this Entity entity, float value)
        {
            entity.SetProperty(FRICTION, value);
        }
        #endregion

        #region 染色
        public static readonly PropertyMeta<Color> TINT = Get<Color>("tint");
        public static Color GetTint(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Color>(TINT, ignoreBuffs: ignoreBuffs);
        }
        public static void SetTint(this Entity entity, Color value)
        {
            entity.SetProperty(TINT, value);
        }
        #endregion

        #region 颜色偏移
        public static readonly PropertyMeta<Color> COLOR_OFFSET = Get<Color>("colorOffset");
        public static Color GetColorOffset(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Color>(COLOR_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public static void SetColorOffset(this Entity entity, Color value)
        {
            entity.SetProperty(COLOR_OFFSET, value);
        }
        #endregion

        #region 阵营
        public static readonly PropertyMeta<int> FACTION = Get<int>("faction");
        public static void SetFaction(this Entity entity, int value)
        {
            entity.SetProperty(FACTION, value);
        }
        #endregion

        #region 地面高度偏移
        public static readonly PropertyMeta<float> GROUND_LIMIT_OFFSET = Get<float>("groundLimitOffset");
        public static float GetGroundLimitOffset(this Entity entity)
        {
            return entity.GetProperty<float>(EngineEntityProps.GROUND_LIMIT_OFFSET);
        }
        #endregion

        #region 碰撞盒中心点
        public static readonly PropertyMeta<Vector3> BOUNDS_PIVOT = Get<Vector3>("boundsPivot");
        public static Vector3 GetBoundsPivot(this EntityDefinition entity)
        {
            return entity.GetProperty<Vector3>(BOUNDS_PIVOT);
        }
        public static Vector3 GetBoundsPivot(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Vector3>(BOUNDS_PIVOT, ignoreBuffs: ignoreBuffs);
        }
        public static void SetBoundsPivot(this Entity entity, Vector3 value)
        {
            entity.SetProperty(BOUNDS_PIVOT, value);
        }
        #endregion

        #region 最大血量
        public static readonly PropertyMeta<float> MAX_HEALTH = Get<float>("maxHealth");
        public static float GetMaxHealth(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
        #endregion

        #region 外壳ID
        public static readonly PropertyMeta<NamespaceID> SHELL = Get<NamespaceID>("shell");
        public static NamespaceID GetShellID(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SHELL);
        }
        public static NamespaceID GetShellID(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<NamespaceID>(SHELL, ignoreBuffs: ignoreBuffs);
        }
        public static void SetShellID(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(SHELL, value);
        }
        #endregion

        #region 放置ID
        public static readonly PropertyMeta<NamespaceID> PLACEMENT = Get<NamespaceID>("placement");
        public static NamespaceID GetPlacementID(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID>(PLACEMENT);
        }
        #endregion

        #region 模型ID
        public static readonly PropertyMeta<NamespaceID> MODEL_ID = Get<NamespaceID>("modelId");
        #endregion

        #region 碰撞检测

        public static readonly PropertyMeta<int> COLLISION_DETECTION = Get<int>("collisionDetection");
        public static int GetCollisionDetection(this Entity entity)
        {
            return entity.GetProperty<int>(COLLISION_DETECTION);
        }
        #endregion

        #region 碰撞检测样本长度
        public static readonly PropertyMeta<float> COLLISION_SAMPLE_LENGTH = Get<float>("collisionSampleLength");
        public static float GetCollisionSampleLength(this Entity entity)
        {
            return entity.GetProperty<float>(COLLISION_SAMPLE_LENGTH);
        }
        #endregion
    }
}

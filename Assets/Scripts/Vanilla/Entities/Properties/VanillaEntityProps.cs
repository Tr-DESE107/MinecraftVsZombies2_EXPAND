using MVZ2Logic;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEntityProps
    {
        #region 射击
        public const string RANGE = "range";
        public const string SHOT_VELOCITY = "shotVelocity";
        public const string SHOT_OFFSET = "shotOffset";
        public const string SHOOT_SOUND = "shootSound";
        public const string PROJECTILE_ID = "projectileId";
        public static void SetRange(this Entity entity, float value)
        {
            entity.SetProperty(RANGE, value);
        }
        public static float GetRange(this Entity entity)
        {
            return entity.GetProperty<float>(RANGE);
        }
        public static Vector3 GetShotVelocity(this Entity entity)
        {
            return entity.GetProperty<Vector3>(SHOT_VELOCITY);
        }
        public static Vector3 GetShotOffset(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<Vector3>(SHOT_OFFSET, ignoreBuffs : ignoreBuffs);
        }
        public static NamespaceID GetShootSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(SHOOT_SOUND);
        }
        public static NamespaceID GetProjectileID(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PROJECTILE_ID);
        }
        #endregion

        #region 攻击
        public const string DAMAGE = "damage";
        public const string ATTACK_SPEED = "attackSpeed";

        public static float GetDamage(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(DAMAGE, ignoreBuffs: ignoreBuffs);
        }
        public static void SetDamage(this Entity entity, float value)
        {
            entity.SetProperty(DAMAGE, value);
        }
        public static float GetAttackSpeed(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<float>(ATTACK_SPEED, ignoreBuffs: ignoreBuffs);
        }
        #endregion

        #region 摔落
        public const string FALL_DAMAGE = "fallDamage";
        public static float GetFallDamage(this Entity entity)
        {
            return entity.GetProperty<float>(FALL_DAMAGE);
        }
        public static void SetFallDamage(this Entity entity, float value)
        {
            entity.SetProperty(FALL_DAMAGE, value);
        }
        #endregion

        #region 沉没
        public const string WATER_INTERACTION = "waterInteraction";
        public static int GetWaterInteraction(this Entity entity)
        {
            return entity.GetProperty<int>(WATER_INTERACTION);
        }
        #endregion

        #region 虚无
        public const string ETHEREAL = "ethereal";
        public static bool IsEthereal(this Entity entity)
        {
            return entity.GetProperty<bool>(ETHEREAL);
        }
        #endregion

        #region 生产
        public const string PRODUCE_SPEED = "produceSpeed";
        public static float GetProduceSpeed(this Entity entity)
        {
            return entity.GetProperty<float>(PRODUCE_SPEED);
        }
        #endregion

        #region 索敌
        public const string INVISIBLE = "invisible";
        public const string AI_FROZEN = "aiFrozen";

        public static bool IsInvisible(this Entity entity)
        {
            return entity.GetProperty<bool>(INVISIBLE);
        }
        public static bool IsAIFrozen(this Entity entity)
        {
            return entity.GetProperty<bool>(AI_FROZEN);
        }
        #endregion

        #region 音效
        public const string HIT_SOUND = "hitSound";
        public const string DEATH_SOUND = "deathSound";
        public const string PLACE_SOUND = "placeSound";
        public static NamespaceID GetHitSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(HIT_SOUND);
        }
        public static NamespaceID GetPlaceSound(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(PLACE_SOUND);
        }
        public static NamespaceID GetPlaceSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PLACE_SOUND);
        }
        public static NamespaceID GetDeathSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(DEATH_SOUND);
        }
        #endregion

        #region 换行
        public const string CHANGING_LANE = "isChangingLane";
        public const string CHANGE_LANE_SPEED = "changeLaneSpeed";
        public const string CHANGE_LANE_TARGET = "changeLaneTarget";
        public const string CHANGE_LANE_SOURCE = "changeLaneSource";
        #endregion

        #region 显示
        public const string SORTING_LAYER = "sortingLayer";
        public const string SORTING_ORDER = "sortingOrder";
        public static int GetSortingLayer(this Entity entity)
        {
            return entity.GetProperty<int>(VanillaEntityProps.SORTING_LAYER);
        }
        public static void SetSortingLayer(this Entity entity, int layer)
        {
            entity.SetProperty(VanillaEntityProps.SORTING_LAYER, layer);
        }
        public static int GetSortingOrder(this Entity entity)
        {
            return entity.GetProperty<int>(VanillaEntityProps.SORTING_ORDER);
        }
        public static void SetSortingOrder(this Entity entity, int layer)
        {
            entity.SetProperty(VanillaEntityProps.SORTING_ORDER, layer);
        }
        #endregion

        #region 更新
        public const string UPDATE_BEFORE_GAME = "updateBeforeGame";
        public const string UPDATE_AFTER_GAME_OVER = "updateAfterGameOver";
        public static bool CanUpdateBeforeGameStart(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_BEFORE_GAME);
        }
        public static bool CanUpdateAfterGameOver(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_AFTER_GAME_OVER);
        }
        #endregion

        #region 影子

        public const string SHADOW_HIDDEN = "shadowHidden";
        public const string SHADOW_ALPHA = "shadowAlpha";
        public const string SHADOW_SCALE = "shadowScale";
        public const string SHADOW_OFFSET = "shadowOffset";
        public static bool IsShadowHidden(this Entity entity) => entity.GetProperty<bool>(VanillaEntityProps.SHADOW_HIDDEN);
        public static void SetShadowHidden(this Entity entity, bool value) => entity.SetProperty(VanillaEntityProps.SHADOW_HIDDEN, value);
        public static float GetShadowAlpha(this Entity entity) => entity.GetProperty<float>(VanillaEntityProps.SHADOW_ALPHA);
        public static void SetShadowAlpha(this Entity entity, float value) => entity.SetProperty(VanillaEntityProps.SHADOW_ALPHA, value);
        public static Vector3 GetShadowScale(this Entity entity) => entity.GetProperty<Vector3>(VanillaEntityProps.SHADOW_SCALE);
        public static void SetShadowScale(this Entity entity, Vector3 value) => entity.SetProperty(VanillaEntityProps.SHADOW_SCALE, value);
        public static Vector3 GetShadowOffset(this Entity entity) => entity.GetProperty<Vector3>(VanillaEntityProps.SHADOW_OFFSET);
        public static void SetShadowOffset(this Entity entity, Vector3 value) => entity.SetProperty(VanillaEntityProps.SHADOW_OFFSET, value);
        #endregion

        #region 时限
        public const string MAX_TIMEOUT = "maxTimeout";
        public static int GetMaxTimeout(this Entity entity)
        {
            return entity.GetProperty<int>(MAX_TIMEOUT);
        }
        #endregion

        #region 亡灵
        public const string IS_UNDEAD = "undead";
        public static void SetIsUndead(this Entity entity, bool value)
        {
            entity.SetProperty(IS_UNDEAD, value);
        }
        public static bool IsUndead(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_UNDEAD);
        }
        #endregion

        #region 火焰

        public const string IS_FIRE = "isFire";
        public static void SetIsFire(this Entity entity, bool value)
        {
            entity.SetProperty(IS_FIRE, value);
        }
        public static bool IsFire(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_FIRE);
        }
        #endregion

        #region 光照
        public const string IS_LIGHT_SOURCE = "isLightSource";
        public const string LIGHT_RANGE = "lightRange";
        public const string LIGHT_COLOR = "lightColor";
        public static void SetLightSource(this Entity entity, bool value)
        {
            entity.SetProperty(IS_LIGHT_SOURCE, value);
        }
        public static bool IsLightSource(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_LIGHT_SOURCE);
        }
        public static void SetLightRange(this Entity entity, Vector3 value)
        {
            entity.SetProperty(LIGHT_RANGE, value);
        }
        public static Vector3 GetLightRange(this Entity entity)
        {
            return entity.GetProperty<Vector3>(LIGHT_RANGE);
        }
        public static Color GetLightColor(this Entity entity)
        {
            return entity.GetProperty<Color>(LIGHT_COLOR);
        }
        #endregion

        #region 血液
        public const string BLOOD_COLOR = "bloodColor";
        public const string BLOOD_COLOR_CENSORED = "bloodColorCensored";
        public static Color GetBloodColorNormal(this Entity entity)
        {
            return entity.GetProperty<Color>(BLOOD_COLOR);
        }
        public static Color GetBloodColorCensored(this Entity entity)
        {
            return entity.GetProperty<Color>(BLOOD_COLOR_CENSORED);
        }
        public static Color GetBloodColor(this Entity entity)
        {
            return Global.HasBloodAndGore() ? entity.GetBloodColorNormal() : entity.GetBloodColorCensored();
        }
        #endregion

        #region 蓝图
        public const string COST = "cost";
        public const string RECHARGE_ID = "rechargeId";
        public static int GetCost(this Entity entity)
        {
            return entity.GetProperty<int>(COST);
        }
        public static int GetCost(this EntityDefinition entity)
        {
            return entity.GetProperty<int>(COST);
        }
        public static NamespaceID GetRechargeID(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        public static NamespaceID GetRechargeID(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        #endregion

        #region 单元格
        public const string GRID_LAYERS = "gridLayers";
        public static NamespaceID[] GetGridLayersToTake(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID[]>(GRID_LAYERS);
        }
        public static NamespaceID[] GetGridLayersToTake(this Entity entity)
        {
            return entity.GetProperty<NamespaceID[]>(GRID_LAYERS);
        }
        #endregion

        #region 堆叠
        public const string STACK_ON = "stackOn";
        public static NamespaceID GetStackOnEntity(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID>(STACK_ON);
        }
        #endregion

        #region 摩擦力
        public const string KEEP_GROUND_FRICTION = "KeepGroundFriction";
        public static bool KeepGroundFriction(this Entity entity)
        {
            return entity.GetProperty<bool>(KEEP_GROUND_FRICTION);
        }
        #endregion

        #region 忠诚
        public const string LOYAL = "loyal";
        public static bool IsLoyal(this Entity entity)
        {
            return entity.GetProperty<bool>(LOYAL);
        }
        #endregion
    }
}

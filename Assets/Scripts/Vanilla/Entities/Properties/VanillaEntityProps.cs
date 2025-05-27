using MVZ2Logic;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaEntityProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<float> MASS = Get<float>("mass");
        public static void SetMass(this Entity entity, float value)
        {
            entity.SetProperty(MASS, value);
        }
        public static float GetMass(this Entity entity)
        {
            return entity.GetProperty<float>(MASS);
        }
        public static float GetKnockbackMultiplier(this Entity entity, float massMultiplier)
        {
            var mass = entity.GetMass();
            return Mathf.Max(0, 2 - Mathf.Pow(2, mass * massMultiplier));
        }
        public static float GetWeakKnockbackMultiplier(this Entity entity) => entity.GetKnockbackMultiplier(1);
        public static float GetStrongKnockbackMultiplier(this Entity entity) => entity.GetKnockbackMultiplier(0.5f);
        #region 射击
        public static readonly PropertyMeta<float> RANGE = Get<float>("range");
        public static readonly PropertyMeta<Vector3> SHOT_VELOCITY = Get<Vector3>("shotVelocity");
        public static readonly PropertyMeta<Vector3> SHOT_OFFSET = Get<Vector3>("shotOffset");
        public static readonly PropertyMeta<NamespaceID> SHOOT_SOUND = Get<NamespaceID>("shootSound");
        public static readonly PropertyMeta<NamespaceID> PROJECTILE_ID = Get<NamespaceID>("projectileId");
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
            return entity.GetProperty<Vector3>(SHOT_OFFSET, ignoreBuffs: ignoreBuffs);
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
        public static readonly PropertyMeta<float> DAMAGE = Get<float>("damage");
        public static readonly PropertyMeta<float> ATTACK_SPEED = Get<float>("attackSpeed");

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
        public static readonly PropertyMeta<float> FALL_RESISTANCE = Get<float>("fallResistance");
        public static float GetFallResistance(this Entity entity)
        {
            return entity.GetProperty<float>(FALL_RESISTANCE);
        }
        public static void SetFallResistance(this Entity entity, float value)
        {
            entity.SetProperty(FALL_RESISTANCE, value);
        }
        #endregion

        #region 沉没
        public static readonly PropertyMeta<int> WATER_INTERACTION = Get<int>("waterInteraction");
        public static int GetWaterInteraction(this EntityDefinition entityDef)
        {
            return entityDef.GetProperty<int>(WATER_INTERACTION);
        }
        public static int GetWaterInteraction(this Entity entity)
        {
            return entity.GetProperty<int>(WATER_INTERACTION);
        }
        public static void SetWaterInteraction(this Entity entity, int value)
        {
            entity.SetProperty(WATER_INTERACTION, value);
        }
        #endregion

        #region 车辆
        public static readonly PropertyMeta<int> VEHICLE_INTERACTION = Get<int>("vehicleInteraction");
        public static int GetVehicleInteraction(this Entity entity)
        {
            return entity.GetProperty<int>(VEHICLE_INTERACTION);
        }
        #endregion

        #region 虚无
        public static readonly PropertyMeta<bool> ETHEREAL = Get<bool>("ethereal");
        public static bool IsEthereal(this Entity entity)
        {
            return entity.GetProperty<bool>(ETHEREAL);
        }
        #endregion

        #region 生产
        public static readonly PropertyMeta<float> PRODUCE_SPEED = Get<float>("produceSpeed");
        public static float GetProduceSpeed(this Entity entity)
        {
            return entity.GetProperty<float>(PRODUCE_SPEED);
        }
        #endregion

        #region 索敌
        public static readonly PropertyMeta<bool> INVISIBLE = Get<bool>("invisible");
        public static readonly PropertyMeta<bool> AI_FROZEN = Get<bool>("aiFrozen");

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
        public static readonly PropertyMeta<NamespaceID> HIT_SOUND = Get<NamespaceID>("hitSound");
        public static readonly PropertyMeta<NamespaceID> DEATH_SOUND = Get<NamespaceID>("deathSound");
        public static readonly PropertyMeta<NamespaceID> PLACE_SOUND = Get<NamespaceID>("placeSound");
        public static readonly PropertyMeta<float> CRY_PITCH = Get<float>("cryPitch");
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
        public static float GetCryPitch(this Entity entity)
        {
            return entity.GetProperty<float>(CRY_PITCH);
        }
        #endregion

        #region 换行
        public static readonly PropertyMeta<float> CHANGE_LANE_SPEED = Get<float>("changeLaneSpeed");
        public static float GetChangeLaneSpeed(this Entity entity) => entity.GetProperty<float>(VanillaEntityProps.CHANGE_LANE_SPEED);
        #endregion

        #region 显示
        public static readonly PropertyMeta<string> SORTING_LAYER = Get<string>("sortingLayer");
        public static readonly PropertyMeta<int> SORTING_ORDER = Get<int>("sortingOrder");
        public static string GetSortingLayer(this Entity entity)
        {
            return entity.GetProperty<string>(VanillaEntityProps.SORTING_LAYER);
        }
        public static void SetSortingLayer(this Entity entity, string layer)
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
        public static readonly PropertyMeta<bool> UPDATE_BEFORE_GAME = Get<bool>("updateBeforeGame");
        public static readonly PropertyMeta<bool> UPDATE_IN_PAUSE = Get<bool>("updateInPause");
        public static readonly PropertyMeta<bool> UPDATE_AFTER_GAME_OVER = Get<bool>("updateAfterGameOver");
        public static void SetCanUpdateBeforeGameStart(this Entity entity, bool value)
        {
            entity.SetProperty(UPDATE_BEFORE_GAME, value);
        }
        public static bool CanUpdateBeforeGameStart(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_BEFORE_GAME);
        }
        public static bool CanUpdateInPause(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_IN_PAUSE);
        }
        public static bool CanUpdateAfterGameOver(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_AFTER_GAME_OVER);
        }
        #endregion

        #region 影子

        public static readonly PropertyMeta<bool> SHADOW_HIDDEN = Get<bool>("shadowHidden");
        public static readonly PropertyMeta<float> SHADOW_ALPHA = Get<float>("shadowAlpha");
        public static readonly PropertyMeta<Vector3> SHADOW_SCALE = Get<Vector3>("shadowScale");
        public static readonly PropertyMeta<Vector3> SHADOW_OFFSET = Get<Vector3>("shadowOffset");
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
        public static readonly PropertyMeta<int> MAX_TIMEOUT = Get<int>("maxTimeout");
        public static int GetMaxTimeout(this Entity entity)
        {
            return entity.GetProperty<int>(MAX_TIMEOUT);
        }
        #endregion

        #region 亡灵
        public static readonly PropertyMeta<bool> IS_UNDEAD = Get<bool>("undead");
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

        public static readonly PropertyMeta<bool> IS_FIRE = Get<bool>("isFire");
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
        public static readonly PropertyMeta<bool> IS_LIGHT_SOURCE = Get<bool>("isLightSource");
        public static readonly PropertyMeta<bool> RECEIVES_LIGHT = Get<bool>("receivesLight");
        public static readonly PropertyMeta<Vector3> LIGHT_RANGE = Get<Vector3>("lightRange");
        public static readonly PropertyMeta<Color> LIGHT_COLOR = Get<Color>("lightColor");
        public static void SetLightSource(this Entity entity, bool value)
        {
            entity.SetProperty(IS_LIGHT_SOURCE, value);
        }
        public static bool IsLightSource(this Entity entity)
        {
            return entity.GetProperty<bool>(IS_LIGHT_SOURCE);
        }
        public static void SetReceivesLight(this Entity entity, bool value)
        {
            entity.SetProperty(RECEIVES_LIGHT, value);
        }
        public static bool ReceivesLight(this Entity entity)
        {
            return entity.GetProperty<bool>(RECEIVES_LIGHT);
        }
        public static void SetLightRange(this Entity entity, Vector3 value)
        {
            entity.SetProperty(LIGHT_RANGE, value);
        }
        public static Vector3 GetLightRange(this Entity entity)
        {
            return entity.GetProperty<Vector3>(LIGHT_RANGE);
        }
        public static void SetLightColor(this Entity entity, Color value)
        {
            entity.SetProperty(LIGHT_COLOR, value);
        }
        public static Color GetLightColor(this Entity entity)
        {
            return entity.GetProperty<Color>(LIGHT_COLOR);
        }
        #endregion

        #region 血液
        public static readonly PropertyMeta<Color> BLOOD_COLOR = Get<Color>("bloodColor");
        public static readonly PropertyMeta<Color> BLOOD_COLOR_CENSORED = Get<Color>("bloodColorCensored");
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

        #region HSV
        public static readonly PropertyMeta<Vector3> HSV = Get<Vector3>("hsv");
        public static void SetHSV(this Entity entity, float h, float s, float v) => entity.SetHSV(new Vector3(h, s, v));
        public static void SetHSV(this Entity entity, Vector3 value) => entity.SetProperty(HSV, value);
        public static Vector3 GetHSV(this Entity entity) => entity.GetProperty<Vector3>(HSV);
        public static void SetHSVToColor(this Entity entity, Color srcColor)
        {
            entity.SetHSVToColor(srcColor, Color.red);
        }
        public static void SetHSVToColor(this Entity entity, Color srcColor, Color dstColor)
        {
            Color.RGBToHSV(srcColor, out var srcH, out var srcS, out var srcV);
            Color.RGBToHSV(dstColor, out var dstH, out var dstS, out var dstV);
            var h = (srcH - dstH) * 360;
            var s = (srcS - dstS) * 100;
            var v = (srcV - dstV) * 100;
            entity.SetHSV(h, s, v);
        }
        #endregion

        #region 灰度
        public static readonly PropertyMeta<bool> GRAYSCALE = Get<bool>("grayscale");
        public static void SetGrayscale(this Entity entity, bool value) => entity.SetProperty(GRAYSCALE, value);
        public static bool IsGrayscale(this Entity entity) => entity.GetProperty<bool>(GRAYSCALE);
        #endregion

        #region 蓝图
        public static readonly PropertyMeta<int> COST = Get<int>("cost");
        public static readonly PropertyMeta<NamespaceID> RECHARGE_ID = Get<NamespaceID>("rechargeId");
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
        public static readonly PropertyMeta<NamespaceID[]> GRID_LAYERS = Get<NamespaceID[]>("gridLayers");
        public static NamespaceID[] GetGridLayersToTake(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID[]>(GRID_LAYERS);
        }
        public static NamespaceID[] GetGridLayersToTake(this Entity entity)
        {
            return entity.GetProperty<NamespaceID[]>(GRID_LAYERS);
        }
        public static void SetGridLayersToTake(this Entity entity, NamespaceID[] value)
        {
            entity.SetProperty(GRID_LAYERS, value);
        }
        #endregion

        #region 升级
        public static readonly PropertyMeta<NamespaceID> UPGRADE_FROM = Get<NamespaceID>("upgradeFrom");
        public static NamespaceID GetUpgradeFromEntity(this EntityDefinition entity)
        {
            return entity.GetProperty<NamespaceID>(UPGRADE_FROM);
        }
        #endregion

        #region 摩擦力
        public static readonly PropertyMeta<bool> KEEP_GROUND_FRICTION = Get<bool>("KeepGroundFriction");
        public static bool KeepGroundFriction(this Entity entity)
        {
            return entity.GetProperty<bool>(KEEP_GROUND_FRICTION);
        }
        #endregion

        #region 忠诚
        public static readonly PropertyMeta<bool> LOYAL = Get<bool>("loyal");
        public static bool IsLoyal(this Entity entity)
        {
            return entity.GetProperty<bool>(LOYAL);
        }
        #endregion
        public static readonly PropertyMeta<bool> NO_HELD_TARGET = Get<bool>("noHeldTarget");
        public static bool NoHeldTarget(this Entity entity)
        {
            return entity.GetProperty<bool>(NO_HELD_TARGET);
        }


        public static readonly PropertyMeta<float> TAKEN_CRUSH_DAMAGE = Get<float>("takenCrushDamage");
        public static float GetTakenCrushDamage(this Entity entity)
        {
            return entity.GetProperty<float>(TAKEN_CRUSH_DAMAGE);
        }
    }
}


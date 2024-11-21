using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2Logic.Entities
{
    public static class BuiltinEntity
    {
        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1)
        {
            entity.Level.PlaySound(soundID, entity.Position, pitch);
        }
        public static int GetSortingLayer(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.SORTING_LAYER);
        }
        public static void SetSortingLayer(this Entity entity, int layer)
        {
            entity.SetProperty(BuiltinEntityProps.SORTING_LAYER, layer);
        }
        public static int GetSortingOrder(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.SORTING_ORDER);
        }
        public static void SetSortingOrder(this Entity entity, int layer)
        {
            entity.SetProperty(BuiltinEntityProps.SORTING_ORDER, layer);
        }
        public static NamespaceID GetPlaceSound(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(BuiltinEntityProps.PLACE_SOUND);
        }
        public static NamespaceID GetDeathSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(BuiltinEntityProps.DEATH_SOUND);
        }

        #region 影子
        public static bool IsShadowHidden(this Entity entity) => entity.GetProperty<bool>(BuiltinEntityProps.SHADOW_HIDDEN);
        public static void SetShadowHidden(this Entity entity, bool value) => entity.SetProperty(BuiltinEntityProps.SHADOW_HIDDEN, value);
        public static float GetShadowAlpha(this Entity entity) => entity.GetProperty<float>(BuiltinEntityProps.SHADOW_ALPHA);
        public static void SetShadowAlpha(this Entity entity, float value) => entity.SetProperty(BuiltinEntityProps.SHADOW_ALPHA, value);
        public static Vector3 GetShadowScale(this Entity entity) => entity.GetProperty<Vector3>(BuiltinEntityProps.SHADOW_SCALE);
        public static void SetShadowScale(this Entity entity, Vector3 value) => entity.SetProperty(BuiltinEntityProps.SHADOW_SCALE, value);
        public static Vector3 GetShadowOffset(this Entity entity) => entity.GetProperty<Vector3>(BuiltinEntityProps.SHADOW_OFFSET);
        public static void SetShadowOffset(this Entity entity, Vector3 value) => entity.SetProperty(BuiltinEntityProps.SHADOW_OFFSET, value);
        #endregion
        public static int GetMaxTimeout(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.MAX_TIMEOUT);
        }
        public static void StartChangingLane(this Entity entity, int target)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
                return;
            if (target < 0 || target >= entity.Level.GetMaxLaneCount())
                return;
            changeLane.SetChangingLane(entity, true);
            changeLane.SetChangeLaneTarget(entity, target);
            changeLane.SetChangeLaneSource(entity, entity.GetLane());
            changeLane.PostStartChangingLane(entity, target);
        }
        public static void StopChangingLane(this Entity entity)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
                return;
            if (!changeLane.IsChangingLane(entity))
                return;
            changeLane.SetChangingLane(entity, false);
            changeLane.SetChangeLaneTarget(entity, 0);
            changeLane.SetChangeLaneSource(entity, 0);
            changeLane.PostStopChangingLane(entity);
        }
    }
}

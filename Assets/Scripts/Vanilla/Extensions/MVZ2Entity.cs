using MVZ2.GameContent;
using MVZ2.GameContent.Shells;
using PVZEngine;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.Vanilla
{
    public static class MVZ2Entity
    {
        public static Vector3 GetSize(this EntityDefinition definition)
        {
            return definition.GetProperty<Vector3>(EntityProperties.SIZE);
        }
        public static NamespaceID GetPlaceSound(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(EntityProps.PLACE_SOUND);
        }
        public static NamespaceID GetDeathSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(EntityProps.DEATH_SOUND);
        }

        #region 影子
        public static bool IsShadowHidden(this Entity entity) => entity.GetProperty<bool>(EntityProps.SHADOW_HIDDEN);
        public static void SetShadowHidden(this Entity entity, bool value) => entity.SetProperty(EntityProps.SHADOW_HIDDEN, value);
        public static float GetShadowAlpha(this Entity entity) => entity.GetProperty<float>(EntityProps.SHADOW_ALPHA);
        public static void SetShadowAlpha(this Entity entity, float value) => entity.SetProperty(EntityProps.SHADOW_ALPHA, value);
        public static Vector3 GetShadowScale(this Entity entity) => entity.GetProperty<Vector3>(EntityProps.SHADOW_SCALE);
        public static void SetShadowScale(this Entity entity, Vector3 value)  => entity.SetProperty(EntityProps.SHADOW_SCALE, value);
        public static Vector3 GetShadowOffset(this Entity entity) => entity.GetProperty<Vector3>(EntityProps.SHADOW_OFFSET);
        public static void SetShadowOffset(this Entity entity, Vector3 value) => entity.SetProperty(EntityProps.SHADOW_OFFSET, value);
        #endregion
        public static int GetMaxTimeout(this Entity entity)
        {
            return entity.GetProperty<int>(EntityProps.MAX_TIMEOUT);
        }
        public static void StartChangingLane(this Entity entity, int target)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
                return;
            if (target < 0 || target >= entity.Game.GetMaxLaneCount())
                return;
            changeLane.SetChangeLaneTarget(entity, target);
            changeLane.SetChangeLaneSource(entity, entity.GetLane());
            changeLane.PostStartChangingLane(entity, target);
        }
        public static void StopChangingLane(this Entity entity)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
                return;
            if (changeLane.GetChangeLaneTarget(entity) < 0)
                return;
            changeLane.SetChangeLaneTarget(entity, -1);
            changeLane.SetChangeLaneSource(entity, -1);
            changeLane.PostStopChangingLane(entity);
        }
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Game;
            var blocksFire = shell.GetProperty<bool>(ShellProps.BLOCKS_FIRE);
            var hitSound = shell.GetProperty<NamespaceID>(ShellProps.HIT_SOUND);
            if (damageEffects.HasEffect(DamageEffects.FIRE) && !blocksFire)
            {
                level.PlaySound(SoundID.fire, entity.Pos);
            }
            else if (damageEffects.HasEffect(DamageEffects.SLICE) && shell.GetProperty<bool>(ShellProps.SLICE_CRITICAL))
            {
                level.PlaySound(SoundID.slice, entity.Pos);
            }
            else
            {
                level.PlaySound(hitSound, entity.Pos);
            }
        }
    }
}

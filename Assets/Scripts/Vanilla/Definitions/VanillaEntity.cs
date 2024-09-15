using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Effects;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEntity : EntityDefinition, IChangeLaneEntity
    {
        #region 公有方法
        protected VanillaEntity(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinEntityProps.PLACE_SOUND, SoundID.grass);
            SetProperty(BuiltinEntityProps.CHANGE_LANE_SPEED, 2.5f);
            SetProperty(BuiltinEntityProps.SHADOW_ALPHA, 1f);
            SetProperty(BuiltinEntityProps.SHADOW_SCALE, Vector3.one);
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (armorResult != null)
            {
                var armor = armorResult.Armor;
                if (armor != null && !armor.HasBuff<ArmorDamageColorBuff>())
                    armor.AddBuff<ArmorDamageColorBuff>();
            }
        }
        public override void PostDestroyArmor(Entity entity, Armor armor, DamageResult damage)
        {
            base.PostDestroyArmor(entity, armor, damage);
            entity.RemoveArmor();
            var effect = entity.Level.Spawn<BrokenArmor>(GetArmorPosition(entity), entity);
            var sourcePosition = damage?.Source?.GetEntity(entity.Level)?.Pos;
            var moveDirection = entity.IsFacingLeft() ? Vector3.right : Vector3.left;
            if (sourcePosition.HasValue)
            {
                moveDirection = (entity.Pos - sourcePosition.Value).normalized;
            }
            effect.Velocity = moveDirection * 10;
            effect.ChangeModel(armor.Definition.GetModelID());
        }
        #endregion

        #region 私有方法

        #region 换行
        public bool IsChangingLane(Entity entity)
        {
            return entity.GetProperty<bool>(BuiltinEntityProps.CHANGING_LANE);
        }
        public void SetChangingLane(Entity entity, bool value)
        {
            entity.SetProperty(BuiltinEntityProps.CHANGING_LANE, value);
        }
        public int GetChangeLaneTarget(Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.CHANGE_LANE_TARGET);
        }
        public void SetChangeLaneTarget(Entity entity, int value)
        {
            entity.SetProperty(BuiltinEntityProps.CHANGE_LANE_TARGET, value);
        }
        public int GetChangeLaneSource(Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.CHANGE_LANE_SOURCE);
        }
        public void SetChangeLaneSource(Entity entity, int value)
        {
            entity.SetProperty(BuiltinEntityProps.CHANGE_LANE_SOURCE, value);
        }
        public float GetChangeLaneSpeed(Entity entity)
        {
            return entity.GetProperty<float>(BuiltinEntityProps.CHANGE_LANE_SPEED);
        }
        public virtual void PostStartChangingLane(Entity entity, int target)
        {
        }
        public virtual void PostStopChangingLane(Entity entity)
        {
        }
        #endregion
        protected virtual Vector3 GetArmorPosition(Entity entity)
        {
            var bounds = entity.GetBounds();
            return bounds.center + Vector3.up * bounds.extents.y;
        }
        protected void UpdateTakenGrids(Entity entity)
        {
            if (entity.GetRelativeY() > leaveGridHeight || entity.Removed)
            {
                entity.ClearTakenGrids();
            }
            else
            {
                foreach (var grid in GetGridsToTake(entity))
                {
                    entity.TakeGrid(grid);
                }
            }
        }
        protected virtual IEnumerable<LawnGrid> GetGridsToTake(Entity entity)
        {
            yield return entity.Level.GetGrid(entity.GetColumn(), entity.GetLane());
        }
        #endregion

        private const float leaveGridHeight = 64;
    }
}

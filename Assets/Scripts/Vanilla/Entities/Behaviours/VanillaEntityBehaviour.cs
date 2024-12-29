using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Armors;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2Logic.Models;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaEntityBehaviour : EntityBehaviourDefinition, IChangeLaneEntity
    {
        #region 公有方法
        protected VanillaEntityBehaviour(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaEntityProps.SORTING_LAYER, SortingLayers.entities);
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var armorResult = result.ArmorResult;
            if (armorResult != null)
            {
                var armor = armorResult.Armor;
                if (armor != null && !armor.HasBuff<ArmorDamageColorBuff>())
                    armor.AddBuff<ArmorDamageColorBuff>();
            }
        }
        public override void PostDestroyArmor(Entity entity, Armor armor, ArmorDamageResult result)
        {
            base.PostDestroyArmor(entity, armor, result);
            entity.RemoveArmor();
            var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, GetArmorPosition(entity), entity);
            var sourcePosition = result?.Source?.GetEntity(entity.Level)?.Position;
            var moveDirection = entity.GetFacingDirection();
            if (sourcePosition.HasValue)
            {
                moveDirection = (entity.Position - sourcePosition.Value).normalized;
            }
            effect.Velocity = moveDirection * 10;
            effect.ChangeModel(armor.Definition.GetModelID());
        }
        #endregion

        #region 私有方法

        #region 换行
        public bool IsChangingLane(Entity entity)
        {
            return entity.GetProperty<bool>(VanillaEntityProps.CHANGING_LANE);
        }
        public void SetChangingLane(Entity entity, bool value)
        {
            entity.SetProperty(VanillaEntityProps.CHANGING_LANE, value);
        }
        public int GetChangeLaneTarget(Entity entity)
        {
            return entity.GetProperty<int>(VanillaEntityProps.CHANGE_LANE_TARGET);
        }
        public void SetChangeLaneTarget(Entity entity, int value)
        {
            entity.SetProperty(VanillaEntityProps.CHANGE_LANE_TARGET, value);
        }
        public int GetChangeLaneSource(Entity entity)
        {
            return entity.GetProperty<int>(VanillaEntityProps.CHANGE_LANE_SOURCE);
        }
        public void SetChangeLaneSource(Entity entity, int value)
        {
            entity.SetProperty(VanillaEntityProps.CHANGE_LANE_SOURCE, value);
        }
        public float GetChangeLaneSpeed(Entity entity)
        {
            return entity.GetProperty<float>(VanillaEntityProps.CHANGE_LANE_SPEED);
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
                    foreach (var layer in entity.GetGridLayersToTake())
                    {
                        entity.TakeGrid(grid, layer);
                    }
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

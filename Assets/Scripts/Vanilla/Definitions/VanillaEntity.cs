using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Effects;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEntity : EntityDefinition
    {
        protected VanillaEntity(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.grass);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.EquipedArmor != null)
                entity.EquipedArmor.RemoveBuffs(entity.EquipedArmor.GetBuffs<ArmorDamageColorBuff>());
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
            var effect = entity.Game.Spawn<BrokenArmor>(GetArmorPosition(entity), entity);
            var sourcePosition = damage?.Source?.GetEntity(entity.Game)?.Pos;
            var moveDirection = entity.IsFacingLeft() ? Vector3.right : Vector3.left;
            if (sourcePosition.HasValue)
            {
                moveDirection = (entity.Pos - sourcePosition.Value).normalized;
            }
            effect.Velocity = moveDirection * 10;
            effect.ChangeModel(armor.Definition.GetModelID());
        }
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
        protected virtual IEnumerable<int> GetGridsToTake(Entity entity)
        {
            yield return entity.Game.GetGridIndex(entity.GetColumn(), entity.GetLane());
        }
        private const float leaveGridHeight = 64;
    }
}

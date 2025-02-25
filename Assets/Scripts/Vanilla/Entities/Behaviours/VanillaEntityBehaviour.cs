using MVZ2.GameContent.Buffs.Armors;
using MVZ2.GameContent.Effects;
using MVZ2Logic.Models;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaEntityBehaviour : EntityBehaviourDefinition
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
            if (armorResult != null && armorResult.Amount > 0)
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
            effect.SetDisplayScale(entity.GetDisplayScale());
            effect.ChangeModel(armor.Definition.GetModelID());
        }
        #endregion

        #region 私有方法
        protected virtual Vector3 GetArmorPosition(Entity entity)
        {
            var bounds = entity.GetBounds();
            return bounds.center + Vector3.up * bounds.extents.y;
        }
        #endregion
    }
}

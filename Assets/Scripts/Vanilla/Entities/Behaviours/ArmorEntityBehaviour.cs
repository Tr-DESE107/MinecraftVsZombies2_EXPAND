using MVZ2.GameContent.Buffs.Armors;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.armorEntity)]
    public class ArmorEntityBehaviour : EntityBehaviourDefinition
    {
        #region 公有方法
        public ArmorEntityBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var armorResult = result.ArmorResult;
            if (armorResult != null && armorResult.Amount > 0 && !armorResult.HasEffect(VanillaDamageEffects.NO_DAMAGE_BLINK))
            {
                var armor = armorResult.Armor;
                armor.DamageBlink();
            }

            var shieldResult = result.ShieldResult;
            if (shieldResult != null && shieldResult.Amount > 0 && !shieldResult.HasEffect(VanillaDamageEffects.NO_DAMAGE_BLINK))
            {
                var shield = shieldResult.Armor;
                shield.DamageBlink();
            }
        }
        public override void PostDestroyArmor(Entity entity, NamespaceID slot, Armor armor, ArmorDestroyInfo result)
        {
            base.PostDestroyArmor(entity, slot, armor, result);
            entity.RemoveArmor(slot);

            if (!armor.HasNoDiscard())
            {
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
        }
        #endregion

        #region 私有方法
        protected Vector3 GetArmorPosition(Entity entity)
        {
            var bounds = entity.GetBounds();
            return bounds.center + Vector3.up * bounds.extents.y;
        }
        #endregion
    }
}

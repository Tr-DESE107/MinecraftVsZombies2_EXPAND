using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

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
                var armorID = armor.Definition.GetID();
                var position = entity.GetArmorDisplayPosition(slot, armorID);
                var displayScale = entity.GetArmorDisplayScale(slot, armorID);

                var spawnParam = entity.GetSpawnParams();
                spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, displayScale);
                var effect = entity.Spawn(VanillaEffectID.brokenArmor, position, spawnParam);

                var sourcePosition = result?.Source?.GetEntity(entity.Level)?.Position;
                var moveDirection = entity.GetFacingDirection();
                if (sourcePosition.HasValue)
                {
                    moveDirection = (entity.Position - sourcePosition.Value).normalized;
                }
                effect.Velocity = moveDirection * 10;

                effect.ChangeModel(armor.Definition.GetModelID());
            }
        }
        #endregion
    }
}

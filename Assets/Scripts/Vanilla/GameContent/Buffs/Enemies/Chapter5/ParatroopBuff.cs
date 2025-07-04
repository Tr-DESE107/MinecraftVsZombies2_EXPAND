using MVZ2.GameContent.Armors;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.paratroop)]
    public class ParatroopBuff : BuffDefinition
    {
        public ParatroopBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.FALL_RESISTANCE, NumberOperator.Add, 10000));
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Multiply, 0));
            AddModifier(new BooleanModifier(VanillaEnemyProps.HANDS_UP, true));
            AddTrigger(LevelCallbacks.POST_ENTITY_CONTACT_GROUND, PostContactGroundCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            entity.DeactivateArmorColliders(VanillaArmorSlots.shield);
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            entity.ActivateArmorColliders(VanillaArmorSlots.shield);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var shield = entity.GetArmorAtSlot(VanillaArmorSlots.shield);
            if (shield == null || shield.Definition.GetID() != VanillaArmorID.umbrellaShield)
            {
                buff.Remove();
                return;
            }
            entity.Velocity = entity.Velocity * 0.7f + Vector3.down * (FALL_SPEED * 0.3f);
        }
        private void PostContactGroundCallback(LevelCallbacks.PostEntityContactGroundParams param, CallbackResult result)
        {
            var entity = param.entity;
            foreach (var buff in entity.GetBuffs<ParatroopBuff>())
            {
                buff.Remove();
            }
        }
        public const float FALL_SPEED = 7f;
    }
}

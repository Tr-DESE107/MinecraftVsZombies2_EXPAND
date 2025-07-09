using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Entities;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [ArmorBehaviourDefinition(VanillaArmorBehaviourNames.umbrellaShield)]
    public class UmbrellaShield : ArmorBehaviourDefinition
    {
        public UmbrellaShield(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
        }
        public override void PostUpdate(Armor armor)
        {
            base.PostUpdate(armor);
            var droop = false;
            if (armor.Owner != null && armor.Owner.State != VanillaEntityStates.ENEMY_PARACHUTE)
            {
                droop = true;
            }
            armor.SetModelProperty("TongueDroop", droop);
        }
        private void PostEntityDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var shield = entity.GetArmorAtSlot(VanillaArmorSlots.shield);
            if (shield == null)
                return;
            if (!shield.Definition.HasBehaviour(this))
                return;
            shield.Destroy();
        }
    }
}

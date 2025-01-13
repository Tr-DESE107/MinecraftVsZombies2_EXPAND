using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaContraptionExt
    {
        public static bool CanEvoke(this Entity contraption)
        {
            var evokable = contraption.Definition.GetBehaviour<IEvokableContraption>();
            if (evokable == null)
                return false;
            return evokable.CanEvoke(contraption);
        }
        public static void Evoke(this Entity contraption)
        {
            var evokable = contraption.Definition.GetBehaviour<IEvokableContraption>();
            if (evokable == null)
                return;
            evokable.Evoke(contraption);
            contraption.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, contraption.GetDefinitionID(), c => c(contraption));
        }
        public static bool HasPassenger(this Entity contraption)
        {
            var grid = contraption?.GetGrid();
            if (grid != null && grid.GetCarrierEntity() == contraption)
            {
                var main = grid.GetMainEntity();
                if (main != null && main.Exists() && main != contraption)
                {
                    return true;
                }
                var protector = grid.GetProtectorEntity();
                if (protector != null && protector.Exists() && protector != contraption)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CanTrigger(this Entity contraption)
        {
            var triggerable = contraption.Definition.GetBehaviour<ITriggerableContraption>();
            if (triggerable == null)
                return false;
            return triggerable.CanTrigger(contraption);
        }
        public static void Trigger(this Entity contraption)
        {
            var triggerable = contraption.Definition.GetBehaviour<ITriggerableContraption>();
            if (triggerable == null)
                return;
            triggerable.Trigger(contraption);
            contraption.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_CONTRAPTION_TRIGGER, c => c(contraption));
        }
        public static void UpgradeToContraption(this Entity contraption, NamespaceID target)
        {
            var grid = contraption.GetGrid();
            if (grid == null)
                return;
            var awake = !contraption.HasBuff<NocturnalBuff>();
            contraption.Remove();
            var upgraded = contraption.GetGrid().PlaceEntity(target);
            if (upgraded == null)
                return;
            if (awake)
            {
                upgraded.RemoveBuffs<NocturnalBuff>();
            }

        }
        public static bool IsEvoked(this Entity contraption)
        {
            return contraption.GetProperty<bool>("Evoked");
        }
        public static void SetEvoked(this Entity contraption, bool value)
        {
            contraption.SetProperty("Evoked", value);
        }

        public static void ShortCircuit(this Entity contraption, int time)
        {
            var buff = contraption.AddBuff<FrankensteinShockedBuff>();
            buff.SetProperty(FrankensteinShockedBuff.PROP_TIMEOUT, time);
        }
    }
}

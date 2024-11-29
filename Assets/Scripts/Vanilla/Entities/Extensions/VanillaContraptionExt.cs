using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2Logic;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Triggers;

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
            contraption.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, contraption.GetDefinitionID(), contraption);
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
            var triggers = Global.Game.GetTriggers(VanillaLevelCallbacks.POST_CONTRAPTION_TRIGGER);
            foreach (var trigger in triggers)
            {
                trigger.Invoke(contraption);
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
        public static void FallIntoWater(this Entity contraption)
        {
            var grid = contraption.GetGrid();
            if (grid.GetTakenEntities().Any(e => e.IsEntityOf(VanillaContraptionID.lilyPad)))
            {
                if (!contraption.GetProperty<bool>(VanillaContraptionProps.PLACE_ON_LILY) && !contraption.IsEntityOf(VanillaContraptionID.lilyPad))
                {
                    contraption.Die();
                }
            }
            else
            {
                if (!contraption.GetProperty<bool>(VanillaContraptionProps.PLACE_ON_WATER))
                {
                    contraption.Remove();
                }
            }
        }
    }
}

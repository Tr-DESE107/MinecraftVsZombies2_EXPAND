using System.Linq;
using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Contraption
    {
        public static void Evoke(this Contraption contraption)
        {
            if (contraption.Definition is IEvokableContraption evokable)
            {
                evokable.Evoke(contraption);
            }
            VanillaCallbacks.PostContraptionEvoked.Run(contraption);
        }
        public static bool IsEvoked(this Contraption contraption)
        {
            return contraption.GetProperty<bool>("Evoked");
        }
        public static void SetEvoked(this Contraption contraption, bool value)
        {
            contraption.SetProperty("Evoked", value);
        }
        public static void FallIntoWater(this Contraption contraption)
        {
            var grid = contraption.GridBelow;
            if (grid.GetTakenEntities().Any(e => e.Definition.GetReference() == ContraptionID.lilyPad))
            {
                if (!contraption.GetProperty<bool>(ContraptionProps.PlaceOnLily) && contraption.Definition.GetReference() != ContraptionID.lilyPad)
                {
                    contraption.Die();
                }
            }
            else
            {
                if (!contraption.GetProperty<bool>(ContraptionProps.PlaceOnWater))
                {
                    contraption.Remove();
                }
            }
        }
    }
}

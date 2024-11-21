using System.Linq;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using PVZEngine.Entities;

namespace MVZ2.Vanilla
{
    public static class VanillaContraptionExt
    {
        public static bool CanEvoke(this Entity contraption)
        {
            if (contraption.Definition is IEvokableContraption evokable)
            {
                return evokable.CanEvoke(contraption);
            }
            return false;
        }
        public static void Evoke(this Entity contraption)
        {
            if (contraption.Definition is IEvokableContraption evokable)
            {
                evokable.Evoke(contraption);
            }
            VanillaLevelCallbacks.PostContraptionEvoked.Run(contraption);
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

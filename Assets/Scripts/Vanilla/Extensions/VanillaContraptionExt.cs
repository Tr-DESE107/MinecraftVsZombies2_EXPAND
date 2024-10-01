using System.Linq;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class VanillaContraptionExt
    {
        public static void FallIntoWater(this Entity contraption)
        {
            var grid = contraption.GetGrid();
            if (grid.GetTakenEntities().Any(e => e.Definition.GetID() == VanillaContraptionID.lilyPad))
            {
                if (!contraption.GetProperty<bool>(VanillaContraptionProps.PLACE_ON_LILY) && contraption.Definition.GetID() != VanillaContraptionID.lilyPad)
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

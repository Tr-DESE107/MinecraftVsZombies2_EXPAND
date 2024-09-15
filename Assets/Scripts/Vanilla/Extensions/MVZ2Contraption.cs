using System.Linq;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class MVZ2Contraption
    {
        public static bool IsFloor(this Entity contraption)
        {
            return contraption.GetProperty<bool>(ContraptionProps.IS_FLOOR);
        }
        public static void FallIntoWater(this Entity contraption)
        {
            var grid = contraption.GetGrid();
            if (grid.GetTakenEntities().Any(e => e.Definition.GetID() == ContraptionID.lilyPad))
            {
                if (!contraption.GetProperty<bool>(ContraptionProps.PLACE_ON_LILY) && contraption.Definition.GetID() != ContraptionID.lilyPad)
                {
                    contraption.Die();
                }
            }
            else
            {
                if (!contraption.GetProperty<bool>(ContraptionProps.PLACE_ON_WATER))
                {
                    contraption.Remove();
                }
            }
        }
    }
}

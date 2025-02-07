using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.any)]
    public class AnyPlacement : PlacementDefinition
    {
        public AnyPlacement(string nsp, string name) : base(nsp, name)
        {
        }
    }
}

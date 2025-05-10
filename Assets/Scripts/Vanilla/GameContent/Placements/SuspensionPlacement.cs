using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.suspension)]
    public class SuspensionPlacement : ContraptionPlacement
    {
        public SuspensionPlacement(string nsp, string name) : base(nsp, name)
        {
        }
    }
}

using MVZ2.Vanilla.Almanacs;
using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.suspension)]
    public class SuspensionPlacement : PlacementDefinition
    {
        public SuspensionPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.suspension)
        {
            AddMethod(VanillaPlaceMethods.entity);
            AddMethod(VanillaPlaceMethods.firstAid);
            this.SetAlmanacTag(VanillaAlmanacTagID.placementSuspension);
        }
    }
}

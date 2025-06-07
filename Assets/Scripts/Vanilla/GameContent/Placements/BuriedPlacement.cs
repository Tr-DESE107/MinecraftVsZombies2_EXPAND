using MVZ2.Vanilla.Almanacs;
using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.buried)]
    public class BuriedPlacement : PlacementDefinition
    {
        public BuriedPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.buried)
        {
            AddMethod(VanillaPlaceMethods.entity);
            AddMethod(VanillaPlaceMethods.firstAid);
            this.SetAlmanacTag(VanillaAlmanacTagID.placementLand);
        }
    }
}

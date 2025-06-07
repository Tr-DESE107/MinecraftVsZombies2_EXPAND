using MVZ2.Vanilla.Almanacs;
using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.forcePad)]
    public class ForcePadPlacement : PlacementDefinition
    {
        public ForcePadPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.pad)
        {
            AddMethod(VanillaPlaceMethods.upgrade);
            this.SetAlmanacTag(VanillaAlmanacTagID.placementLand);
        }
    }
}

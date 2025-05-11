using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.devourer)]
    public class DevourerPlacement : PlacementDefinition
    {
        public DevourerPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.devourer)
        {
            AddMethod(VanillaPlaceMethods.entity);
        }
    }
}

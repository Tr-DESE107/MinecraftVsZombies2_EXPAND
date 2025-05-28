using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.drivenser)]
    public class DrivenserPlacement : PlacementDefinition
    {
        public DrivenserPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.normal)
        {
            AddMethod(VanillaPlaceMethods.entity);
            AddMethod(VanillaPlaceMethods.drivenser);
        }
    }
}

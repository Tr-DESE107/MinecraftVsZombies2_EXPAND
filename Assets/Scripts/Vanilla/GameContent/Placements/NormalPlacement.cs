using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.normal)]
    public class NormalPlacement : PlacementDefinition
    {
        public NormalPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.normal)
        {
            AddMethod(VanillaPlaceMethods.entity);
            AddMethod(VanillaPlaceMethods.firstAid);
        }
    }
}

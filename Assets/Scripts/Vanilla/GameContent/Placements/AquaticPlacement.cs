using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.aquatic)]
    public class AquaticPlacement : PlacementDefinition
    {
        public AquaticPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.aquatic)
        {
            AddMethod(VanillaPlaceMethods.entity);
            AddMethod(VanillaPlaceMethods.firstAid);
        }
    }
}

using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.pad)]
    public class PadPlacement : PlacementDefinition
    {
        public PadPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.pad)
        {
            AddMethod(VanillaPlaceMethods.entity);
        }
    }
}

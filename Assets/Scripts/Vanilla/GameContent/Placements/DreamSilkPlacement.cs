using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.dreamSilk)]
    public class DreamSilkPlacement : PlacementDefinition
    {
        public DreamSilkPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.dreamSilk)
        {
            AddMethod(VanillaPlaceMethods.entity);
        }
    }
}

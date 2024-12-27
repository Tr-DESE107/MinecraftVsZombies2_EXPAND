using MVZ2.Vanilla;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [Definition(VanillaPlacementNames.any)]
    public class AnyPlacement : PlacementDefinition
    {
        public AnyPlacement(string nsp, string name) : base(nsp, name)
        {
        }
    }
}

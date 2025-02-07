using MVZ2.Vanilla.Audios;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.Grids
{
    [GridDefinition(VanillaGridNames.water)]
    public class WaterGrid : GridDefinition
    {
        public WaterGrid(string nsp, string name) : base(nsp, name)
        {
        }

        public override NamespaceID GetPlaceSound(Entity entity)
        {
            return VanillaSoundID.water;
        }
    }
}

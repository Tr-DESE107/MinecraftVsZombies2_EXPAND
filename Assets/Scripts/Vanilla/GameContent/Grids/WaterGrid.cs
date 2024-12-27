using System.Linq;
using System.Threading;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Grids
{
    [Definition(VanillaGridNames.water)]
    public class WaterGrid : GridDefinition, IPlaceSoundGrid
    {
        public WaterGrid(string nsp, string name) : base(nsp, name)
        {
        }

        public NamespaceID GetPlaceSound(Entity entity)
        {
            return VanillaSoundID.water;
        }
    }
}

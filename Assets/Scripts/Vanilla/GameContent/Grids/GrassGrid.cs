using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Grids
{
    [Definition(VanillaGridNames.grass)]
    public class GrassGrid : GridDefinition
    {
        public GrassGrid(string nsp, string name) : base(nsp, name)
        {
        }
    }
}

using System.Linq;
using MVZ2.GameContent.Obstacles;
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
    [Definition(VanillaGridNames.grass)]
    public class GrassGrid : GridDefinition
    {
        public GrassGrid(string nsp, string name) : base(nsp, name)
        {
        }
        public override NamespaceID GetPlaceSound(Entity entity)
        {
            var entitySound = entity.GetPlaceSound();
            if (NamespaceID.IsValid(entitySound))
            {
                return entitySound;
            }
            return VanillaSoundID.grass;
        }
    }
}

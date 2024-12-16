using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Vanilla.Contraptions;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedExt
    {
        public static bool CanInstantTrigger(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.IsTriggerActive() && blueprintDef.CanInstantTrigger();
        }
        public static Entity PlaceSeedEntity(this SeedDefinition seedDef, LawnGrid grid)
        {
            var level = grid.Level;
            var x = level.GetEntityColumnX(grid.Column);
            var z = level.GetEntityLaneZ(grid.Lane);
            var y = level.GetGroundY(x, z);

            var position = new Vector3(x, y, z);
            var entityID = seedDef.GetSeedEntityID();
            var entityDef = level.Content.GetEntityDefinition(entityID);
            return level.Spawn(entityID, position, null);
        }
    }
}

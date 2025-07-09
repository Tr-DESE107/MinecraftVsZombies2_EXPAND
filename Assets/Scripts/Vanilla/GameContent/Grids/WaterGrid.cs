﻿using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Grids;
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
            SetProperty(VanillaGridProps.IS_WATER, true);
        }

        public override NamespaceID GetPlaceSound(Entity entity)
        {
            return VanillaSoundID.water;
        }
    }
}

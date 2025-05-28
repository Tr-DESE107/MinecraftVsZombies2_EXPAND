﻿using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.upgrade)]
    public class UpgradeNormalPlacement : PlacementDefinition
    {
        public UpgradeNormalPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.normal)
        {
            AddMethod(VanillaPlaceMethods.upgrade);
        }
    }
}

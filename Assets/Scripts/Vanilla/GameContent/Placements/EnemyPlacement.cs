﻿using PVZEngine.Level;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.enemy)]
    public class EnemyPlacement : PlacementDefinition
    {
        public EnemyPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.any)
        {
            AddMethod(VanillaPlaceMethods.enemy);
        }
    }
}

﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Obstacles
{
    public static class VanillaObstacleNames
    {
        public const string gargoyleStatue = "gargoyle_statue";
        public const string monsterSpawner = "monster_spawner";
    }
    public static class VanillaObstacleID
    {
        public static readonly NamespaceID gargoyleStatue = Get(VanillaObstacleNames.gargoyleStatue);
        public static readonly NamespaceID monsterSpawner = Get(VanillaObstacleNames.monsterSpawner);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}

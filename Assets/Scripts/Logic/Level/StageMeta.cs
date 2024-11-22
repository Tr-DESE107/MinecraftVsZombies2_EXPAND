using System.Collections.Generic;
using System.Xml;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public class StageMeta
    {
        public string id;
        public string name;
        public int dayNumber;
        public string type;
        public NamespaceID unlock;

        public NamespaceID startTalk;
        public NamespaceID endTalk;
        public NamespaceID mapTalk;

        public NamespaceID clearPickupModel;
        public NamespaceID clearPickupBlueprint;
        public NamespaceID endNote;

        public LevelCameraPosition startCameraPosition;
        public string startTransition;

        public int totalFlags;
        public EnemySpawnEntry[] spawns;
        public int firstWaveTime;

        public Dictionary<string, object> properties;
        public const string TYPE_NORMAL = "normal";
        public const string TYPE_SPECIAL = "special";
        public const string TYPE_MINIGAME = "minigame";
    }
    public class EnemySpawnEntry : IEnemySpawnEntry
    {
        public NamespaceID spawnRef;
        public int earliestFlag;
        public EnemySpawnEntry(NamespaceID spawnRef, int earliestFlag = 0)
        {
            this.spawnRef = spawnRef;
            this.earliestFlag = earliestFlag;
        }

        public bool CanSpawn(LevelEngine game)
        {
            return game.CurrentFlag >= earliestFlag;
        }

        public SpawnDefinition GetSpawnDefinition(IContentProvider game)
        {
            return game.GetSpawnDefinition(spawnRef);
        }
    }
}

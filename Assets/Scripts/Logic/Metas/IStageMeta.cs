using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public interface IStageMeta
    {
        string ID { get; }
        string Name { get; }
        int DayNumber { get; }
        string Type { get; }
        NamespaceID Unlock { get; }

        NamespaceID MusicID { get; }

        NamespaceID StartTalk { get; }
        NamespaceID EndTalk { get; }
        NamespaceID MapTalk { get; }

        NamespaceID ClearPickupModel { get; }
        NamespaceID ClearPickupBlueprint { get; }
        NamespaceID EndNote { get; }

        LevelCameraPosition StartCameraPosition { get; }
        string StartTransition { get; }

        int TotalFlags { get; }
        IEnemySpawnEntry[] Spawns { get; }
        int FirstWaveTime { get; }
        float SpawnPointsMultiplier { get; }

        Dictionary<string, object> Properties { get; }
    }
    public interface IEnemySpawnEntry
    {
        NamespaceID SpawnRef { get; }
        int EarliestFlag { get; }
        bool CanSpawn(LevelEngine game)
        {
            return game.CurrentFlag >= EarliestFlag;
        }

        SpawnDefinition GetSpawnDefinition(IGameContent game)
        {
            return game.GetSpawnDefinition(SpawnRef);
        }
    }
}

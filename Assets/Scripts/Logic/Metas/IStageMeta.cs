using System.Collections.Generic;
using MVZ2Logic.Games;
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

        IStageTalkMeta[] Talks { get; }

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
    public interface IStageTalkMeta
    {
        string Type { get; }
        NamespaceID Value { get; }
        NamespaceID RepeatUntil { get; }
        bool ShouldRepeat(IGameSaveData save)
        {
            return NamespaceID.IsValid(RepeatUntil) && !save.IsUnlocked(RepeatUntil);
        }
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

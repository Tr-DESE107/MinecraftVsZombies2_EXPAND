using System.Collections.Generic;
using System.Linq;
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
        IConveyorPoolEntry[] ConveyorPool { get; }
        int FirstWaveTime { get; }
        bool NeedBlueprints { get; }
        float SpawnPointsMultiplier { get; }

        Dictionary<string, object> Properties { get; }
    }
    public interface IStageTalkMeta
    {
        string Type { get; }
        NamespaceID Value { get; }
        NamespaceID[] RepeatUntil { get; }
        bool ShouldRepeat(IGameSaveData save)
        {
            if (RepeatUntil == null || RepeatUntil.Count(c => NamespaceID.IsValid(c)) <= 0)
                return false;
            return !RepeatUntil.Any(c => save.IsUnlocked(c));
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
    public interface IConveyorPoolEntry
    {
        NamespaceID ID { get; }
        int Count { get; }
    }
}

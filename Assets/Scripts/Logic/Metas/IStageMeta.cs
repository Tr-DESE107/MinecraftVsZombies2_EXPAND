using System.Collections.Generic;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2Logic.Level
{
    public interface IStageMeta
    {
        string ID { get; }
        string Name { get; }
        int DayNumber { get; }
        string Type { get; }
        float StartEnergy { get; }
        NamespaceID Unlock { get; }

        NamespaceID MusicID { get; }

        bool NoStartTalkMusic { get; }
        IStageTalkMeta[] Talks { get; }

        NamespaceID ClearPickupModel { get; }
        NamespaceID ClearPickupBlueprint { get; }
        bool DropsTrophy { get; }
        NamespaceID EndNote { get; }

        LevelCameraPosition StartCameraPosition { get; }
        string StartTransition { get; }

        int TotalFlags { get; }
        NamespaceID[] Spawns { get; }
        IConveyorPoolEntry[] ConveyorPool { get; }
        int FirstWaveTime { get; }
        bool NeedBlueprints { get; }
        float SpawnPointsPower { get; }
        float SpawnPointsMultiplier { get; }
        float SpawnPointsAddition { get; }

        Dictionary<string, object> Properties { get; }
    }
    public interface IStageTalkMeta
    {
        string Type { get; }
        NamespaceID Value { get; }
        int StartSection { get; }
        bool ShouldRepeat(IGameSaveData save);
    }
    public interface IConveyorPoolEntry
    {
        NamespaceID ID { get; }
        int Count { get; }
        int MinCount { get; }
    }
}

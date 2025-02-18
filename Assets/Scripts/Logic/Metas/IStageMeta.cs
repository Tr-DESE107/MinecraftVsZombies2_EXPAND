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

        bool NoStartTalkMusic { get; }
        IStageTalkMeta[] Talks { get; }

        NamespaceID ClearPickupModel { get; }
        NamespaceID ClearPickupBlueprint { get; }
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
        bool ShouldRepeat(IGameSaveData save);
    }
    public interface IConveyorPoolEntry
    {
        NamespaceID ID { get; }
        int Count { get; }
    }
}

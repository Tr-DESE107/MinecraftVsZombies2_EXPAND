using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2Logic.Level
{
    public interface IStageTalkMeta
    {
        string Type { get; }
        NamespaceID Value { get; }
        int StartSection { get; }
        bool ShouldRepeat(IGlobalSaveData save);
    }
    public interface IConveyorPoolEntry
    {
        NamespaceID ID { get; }
        int Count { get; }
        int MinCount { get; }
    }
}

using PVZEngine;

namespace MVZ2Logic.Spawns
{
    public interface IGridLayerMeta
    {
        string ID { get; }
        int Group { get; }
        int Priority { get; }
    }
    public interface IGridErrorMeta
    {
        string ID { get; }
        string Message { get; }
    }
}

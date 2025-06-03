using PVZEngine;

namespace MVZ2Logic.Spawns
{
    public interface IGridLayerMeta
    {
        string ID { get; }
        NamespaceID AlmanacTag { get; }
    }
    public interface IGridErrorMeta
    {
        string ID { get; }
        string Message { get; }
    }
}

using PVZEngine;

namespace MVZ2Logic.Spawns
{
    public interface ISpawnMeta
    {
        string ID { get; }
        NamespaceID Entity { get; }
        int SpawnLevel { get; }
        int MinSpawnWave { get; }
        int PreviewCount { get; }
        ISpawnTerrainMeta Terrain { get; }
        ISpawnWeightMeta Weight { get; }
    }
    public interface ISpawnTerrainMeta
    {
        NamespaceID[] ExcludedAreaTags { get; }
        bool Water { get; }
        bool Air { get; }
    }
    public interface ISpawnWeightMeta
    {
        int Base { get; }
        int DecreaseStart { get; }
        int DecreaseEnd { get; }
        int DecreasePerFlag { get; }
    }
}

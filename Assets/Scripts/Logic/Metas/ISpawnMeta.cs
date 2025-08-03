using PVZEngine;

namespace MVZ2Logic.Spawns
{
    public interface ISpawnMeta
    {
        string ID { get; }
        string Type { get; }
        NamespaceID Entity { get; }
        int EntityVariant { get; }
        NamespaceID PreviewEntity { get; }
        int PreviewVariant { get; }
        int SpawnLevel { get; }
        int MinSpawnWave { get; }
        int PreviewCount { get; }
        bool NoEndless { get; }
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

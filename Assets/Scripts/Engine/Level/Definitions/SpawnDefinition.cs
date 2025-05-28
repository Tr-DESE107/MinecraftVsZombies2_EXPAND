using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class SpawnDefinition : Definition
    {
        public SpawnDefinition(string nsp, string name, int cost, bool noEndless, NamespaceID entityID, NamespaceID[] excludedAreaTags) : base(nsp, name)
        {
            SpawnLevel = cost;
            NoEndless = noEndless;
            EntityID = entityID;
            ExcludedAreaTags = excludedAreaTags;
        }
        public virtual int GetRandomSpawnLane(LevelEngine level)
        {
            return level.GetRandomEnemySpawnLane();
        }
        public abstract int GetWeight(LevelEngine level);
        public int SpawnLevel { get; }
        public bool NoEndless { get; }
        public NamespaceID EntityID { get; }
        public NamespaceID[] ExcludedAreaTags { get; }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.SPAWN;
    }
}

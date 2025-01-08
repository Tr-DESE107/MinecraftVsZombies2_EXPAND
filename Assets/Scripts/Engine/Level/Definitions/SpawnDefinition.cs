using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class SpawnDefinition : Definition
    {
        public SpawnDefinition(string nsp, string name, int cost, NamespaceID entityID, NamespaceID[] excludedAreaTags) : base(nsp, name)
        {
            SpawnLevel = cost;
            EntityID = entityID;
            ExcludedAreaTags = excludedAreaTags;
        }
        public virtual int GetRandomSpawnLane(LevelEngine level)
        {
            return level.GetRandomEnemySpawnLane();
        }
        public abstract int GetWeight(LevelEngine level);
        public int SpawnLevel { get; }
        public NamespaceID EntityID { get; }
        public NamespaceID[] ExcludedAreaTags { get; }
    }
}

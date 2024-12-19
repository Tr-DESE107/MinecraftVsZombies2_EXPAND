using PVZEngine.Base;

namespace PVZEngine.Definitions
{
    public class SpawnDefinition : Definition
    {
        public SpawnDefinition(string nsp, string name, int cost, NamespaceID entityID, NamespaceID[] excludedAreaTags) : base(nsp, name)
        {
            SpawnCost = cost;
            EntityID = entityID;
            ExcludedAreaTags = excludedAreaTags;
        }
        public int SpawnCost { get; }
        public NamespaceID EntityID { get; }
        public NamespaceID[] ExcludedAreaTags { get; }
    }
}

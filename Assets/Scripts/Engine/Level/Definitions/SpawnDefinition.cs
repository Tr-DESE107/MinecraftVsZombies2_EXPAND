namespace PVZEngine.Definitions
{
    public class SpawnDefinition : Definition
    {
        public SpawnDefinition(string nsp, string name, int cost, NamespaceID entityID) : base(nsp, name)
        {
            SpawnCost = cost;
            EntityID = entityID;
        }
        public int SpawnCost { get; }
        public NamespaceID EntityID { get; }
    }
}

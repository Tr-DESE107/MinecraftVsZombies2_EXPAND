namespace PVZEngine.Entities
{
    public class EntityCache
    {
        public void Update(Entity entity)
        {
            Faction = entity.GetFaction();
        }
        public int Faction { get; private set; }
    }
}

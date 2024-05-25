namespace PVZEngine
{
    public class EntityReference
    {
        public EntityReference SpawnerReference { get; private set; }
        public NamespaceID DefinitionID { get; private set; }
        public int ID { get; private set; }

        public EntityReference()
        {

        }
        public EntityReference(Entity entity)
        {
            if (entity != null)
            {
                ID = entity.ID;
                DefinitionID = entity.Definition.GetID();
                SpawnerReference = entity.SpawnerReference;
            }
        }
        public Entity GetEntity(Game game)
        {
            return game.FindEntityByID(ID);
        }
        public override bool Equals(object obj)
        {
            if (obj is EntityReference entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(EntityReference lhs, EntityReference rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityReference lhs, EntityReference rhs)
        {
            return !(lhs == rhs);
        }
    }
}
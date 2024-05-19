namespace PVZEngine
{
    public class EntityReference
    {
        public EntityReference SpawnerReference { get; private set; }
        public Entity Entity { get; private set; }
        public EntityDefinition Definition { get; private set; }
        public int ID { get; private set; }

        public EntityReference()
        {

        }
        public EntityReference(Entity entity)
        {
            ID = entity.ID;
            Entity = entity;
            Definition = entity.Definition;
            SpawnerReference = entity.SpawnerReference;
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
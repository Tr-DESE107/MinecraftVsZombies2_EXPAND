using Newtonsoft.Json;

namespace PVZEngine
{
    public class EntityReference
    {
        public EntityReference()
        {

        }
        public EntityReference(Entity entity)
        {
            if (entity != null)
            {
                id = entity.ID;
                definitionID = entity.Definition.GetID();
                spawnerReference = entity.SpawnerReference?.Clone();
            }
        }
        public EntityReference Clone()
        {
            return new EntityReference
            {
                id = ID,
                definitionID = DefinitionID,
                spawnerReference = SpawnerReference?.Clone()
            };
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
        public EntityReference SpawnerReference => spawnerReference;
        public NamespaceID DefinitionID => definitionID;
        public int ID => id;
        [JsonProperty]
        private EntityReference spawnerReference;
        [JsonProperty]
        private NamespaceID definitionID;
        [JsonProperty]
        private int id;
    }
}
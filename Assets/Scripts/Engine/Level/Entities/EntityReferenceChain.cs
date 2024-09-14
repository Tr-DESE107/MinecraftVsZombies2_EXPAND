using Newtonsoft.Json;

namespace PVZEngine.Level
{
    public class EntityReferenceChain
    {
        public EntityReferenceChain()
        {

        }
        public EntityReferenceChain(Entity entity)
        {
            if (entity != null)
            {
                id = entity.ID;
                definitionID = entity.Definition.GetID();
                spawnerReference = entity.SpawnerReference?.Clone();
            }
        }
        public EntityReferenceChain Clone()
        {
            return new EntityReferenceChain
            {
                id = ID,
                definitionID = DefinitionID,
                spawnerReference = SpawnerReference?.Clone()
            };
        }
        public Entity GetEntity(LevelEngine game)
        {
            return game.FindEntityByID(ID);
        }
        public override bool Equals(object obj)
        {
            if (obj is EntityReferenceChain entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(EntityReferenceChain lhs, EntityReferenceChain rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityReferenceChain lhs, EntityReferenceChain rhs)
        {
            return !(lhs == rhs);
        }
        public EntityReferenceChain SpawnerReference => spawnerReference;
        public NamespaceID DefinitionID => definitionID;
        public int ID => id;
        [JsonProperty]
        private EntityReferenceChain spawnerReference;
        [JsonProperty]
        private NamespaceID definitionID;
        [JsonProperty]
        private int id;
    }
}